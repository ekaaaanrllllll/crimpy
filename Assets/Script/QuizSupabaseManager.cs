using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuizSupabase;

public class QuizSupabaseManager : MonoBehaviour
{
    [Header("Supabase API Config")]
    [SerializeField] private string supabaseUrl = "https://YOUR_PROJECT_ID.supabase.co";
    [SerializeField] private string supabaseApiKey = "YOUR_ANON_KEY";

    [Header("Pengaturan Pertemuan Kuis")]
    [Tooltip("Isi angka pertemuan untuk scene ini. Contoh: 2 untuk Pertemuan 2")]
    public int nomorPertemuanSceneIni = 2; 

    [Header("UI Login References")]
    public GameObject panelLogin;
    public TMP_InputField inputNama;
    public TMP_InputField inputKelas;

    [Header("UI Game Kuis References")]
    public GameObject panelGameKuis;
    public TMP_Text textPertanyaan;
    public TMP_Text[] textChoices; // Taruh 4 Text Komponen (Teks_A s/d Teks_D)
    public Button[] buttonChoices; // Taruh 4 Button Komponen (Tombol_A s/d Tombol_D)

    // State internal kuis
    private int currentStudentId;
    private int selectedMeetingId; 
    private List<Question> activeQuestions = new List<Question>();
    private List<Choice> activeChoices = new List<Choice>();
    
    private int currentQuestionIndex = 0;
    private int totalJawabanBenar = 0;
    private Dictionary<int, Choice> userSelectedChoices = new Dictionary<int, Choice>();

    void OnEnable()
    {
        // Posisi awal panel saat slide dibuka
        panelLogin.SetActive(true);
        panelGameKuis.SetActive(false);

        currentQuestionIndex = 0;
        totalJawabanBenar = 0;
        userSelectedChoices.Clear();
    }

    // ==========================================
    // 1. PROSES LOGIN (CEK DULU BARU POST JIKA BELUM ADA)
    // ==========================================
    public void AmbilDataInputLogin()
    {
        string namaSiswa = inputNama.text.Trim();
        string kelasSiswa = inputKelas.text.Trim();

        if (string.IsNullOrEmpty(namaSiswa) || string.IsNullOrEmpty(kelasSiswa))
        {
            Debug.LogError("Nama dan Kelas wajib diisi, brow!");
            return;
        }

        StartCoroutine(CekAtauDaftarStudent(namaSiswa, kelasSiswa));
    }

    IEnumerator CekAtauDaftarStudent(string nama, string kelas)
    {
        // Jalankan pengecekan data kembar menggunakan query EQ (Equal) di Supabase
        // Menggunakan url encode agar spasi atau karakter unik pada nama/kelas tidak bikin URL error
        string escapedNama = UnityWebRequest.EscapeURL(nama);
        string escapedKelas = UnityWebRequest.EscapeURL(kelas);
        string checkUrl = $"{supabaseUrl}/rest/v1/students?name=eq.{escapedNama}&class_name=eq.{escapedKelas}&select=id";

        using (UnityWebRequest checkRequest = UnityWebRequest.Get(checkUrl))
        {
            checkRequest.SetRequestHeader("apikey", supabaseApiKey);
            checkRequest.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");

            yield return checkRequest.SendWebRequest();

            if (checkRequest.result == UnityWebRequest.Result.Success)
            {
                string checkResponse = checkRequest.downloadHandler.text;

                // Jika respon tidak kosong dan tidak cuma berupa array kosong "[]"
                if (!string.IsNullOrEmpty(checkResponse) && checkResponse != "[]")
                {
                    // Bersihkan tanda urung siku array JSON dari Supabase
                    string cleanJson = checkResponse.TrimStart('[').TrimEnd(']');
                    StudentResponse existingStudent = JsonUtility.FromJson<StudentResponse>(cleanJson);

                    currentStudentId = existingStudent.id;
                    Debug.Log($"<color=cyan>[SUPABASE INFO]</color> Siswa lama ditemukan! Menggunakan ID yang sudah ada: {currentStudentId}");
                    
                    // Langsung masuk ke game kuis tanpa bikin baris baru di DB
                    MasukKeGameKuis();
                    yield break; 
                }
            }
            else
            {
                Debug.LogWarning($"Pengecekan siswa gagal/error, mencoba mendaftar langsung: {checkRequest.error}");
            }
        }

        // ----------------------------------------------------
        // KONDISI B: JIKA SISWA BELUM ADA, MAKA BUAT DATA BARU
        // ----------------------------------------------------
        Debug.Log("[SUPABASE] Siswa tidak ditemukan. Mendaftarkan sebagai siswa baru...");
        string postUrl = $"{supabaseUrl}/rest/v1/students";
        
        Student newStudent = new Student { name = nama, class_name = kelas };
        string jsonPayload = JsonUtility.ToJson(newStudent);

        using (UnityWebRequest request = new UnityWebRequest(postUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Prefer", "return=representation"); 

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text.TrimStart('[').TrimEnd(']');
                StudentResponse savedStudent = JsonUtility.FromJson<StudentResponse>(responseJson);
                
                currentStudentId = savedStudent.id;
                Debug.Log($"<color=green>[SUPABASE SUCCESS]</color> Siswa Baru Berhasil Terdaftar! ID Baru: {currentStudentId}");

                MasukKeGameKuis();
            }
            else
            {
                Debug.LogError($"<color=red>[SUPABASE ERROR]</color> Gagal total memproses siswa!");
                Debug.LogError($"Detail Pesan: {request.downloadHandler.text}");
            }
        }
    }

    void MasukKeGameKuis()
    {
        panelLogin.SetActive(false);
        panelGameKuis.SetActive(true);
        StartCoroutine(GetMeetingIdThenFetchQuestions());
    }

    // ==========================================
    // 2. LOAD DATA SOAL BERDASARKAN PERTEMUAN
    // ==========================================
    IEnumerator GetMeetingIdThenFetchQuestions()
    {
        string url = $"{supabaseUrl}/rest/v1/meetings?meeting_number=eq.{nomorPertemuanSceneIni}&select=id";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text.TrimStart('[').TrimEnd(']');
                
                if (!string.IsNullOrEmpty(responseJson))
                {
                    Meeting targetMeeting = JsonUtility.FromJson<Meeting>(responseJson);
                    selectedMeetingId = targetMeeting.id;
                    
                    StartCoroutine(FetchQuestionsAndChoices(selectedMeetingId));
                }
                else
                {
                    Debug.LogError($"Pertemuan ke-{nomorPertemuanSceneIni} tidak ada di database!");
                }
            }
        }
    }

    IEnumerator FetchQuestionsAndChoices(int meetingId)
    {
        string qUrl = $"{supabaseUrl}/rest/v1/questions?meeting_id=eq.{meetingId}&order=question_order.asc";
        using (UnityWebRequest qRequest = UnityWebRequest.Get(qUrl))
        {
            qRequest.SetRequestHeader("apikey", supabaseApiKey);
            qRequest.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");
            yield return qRequest.SendWebRequest();

            if (qRequest.result == UnityWebRequest.Result.Success)
            {
                string fixJson = "{\"data\":" + qRequest.downloadHandler.text + "}";
                activeQuestions = JsonUtility.FromJson<QuestionList>(fixJson).data;
            }
        }

        string cUrl = $"{supabaseUrl}/rest/v1/choices?select=*";
        using (UnityWebRequest cRequest = UnityWebRequest.Get(cUrl))
        {
            cRequest.SetRequestHeader("apikey", supabaseApiKey);
            cRequest.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");
            yield return cRequest.SendWebRequest();

            if (cRequest.result == UnityWebRequest.Result.Success)
            {
                string fixJson = "{\"data\":" + cRequest.downloadHandler.text + "}";
                activeChoices = JsonUtility.FromJson<ChoiceList>(fixJson).data;
            }
        }

        TampilkanSoalKeUI();
    }

    // ==========================================
    // 3. TAMPILKAN SOAL DAN LOGIKA TOMBOL JAWABAN
    // ==========================================
    void TampilkanSoalKeUI()
    {
        if (activeQuestions.Count == 0)
        {
            textPertanyaan.text = "Tidak ada soal untuk pertemuan ini di database, brow.";
            return;
        }

        Question currentQuestion = activeQuestions[currentQuestionIndex];
        textPertanyaan.text = currentQuestion.question_text;

        List<Choice> filteredChoices = activeChoices.FindAll(c => c.question_id == currentQuestion.id);

        for (int i = 0; i < 4; i++)
        {
            if (i < filteredChoices.Count)
            {
                buttonChoices[i].gameObject.SetActive(true);
                Choice choiceData = filteredChoices[i];
                textChoices[i].text = $"{choiceData.choice_label}.\n{choiceData.choice_text}";
                
                buttonChoices[i].onClick.RemoveAllListeners();
                buttonChoices[i].onClick.AddListener(() => JawabSoal(choiceData));
            }
            else
            {
                buttonChoices[i].gameObject.SetActive(false);
            }
        }
    }

    void JawabSoal(Choice pilihanYangDipilih)
    {
        userSelectedChoices[currentQuestionIndex] = pilihanYangDipilih;

        if (pilihanYangDipilih.is_correct) totalJawabanBenar++;

        if (currentQuestionIndex < activeQuestions.Count - 1)
        {
            currentQuestionIndex++;
            TampilkanSoalKeUI(); 
        }
        else
        {
            StartCoroutine(PostQuizAttemptAndAnswers());
        }
    }

    // ==========================================
    // 4. OTOMATIS POST DATA SETELAH SOAL TERAKHIR
    // ==========================================
    IEnumerator PostQuizAttemptAndAnswers()
    {
        panelGameKuis.SetActive(false); 
        Debug.Log("[PROCESS] Soal habis! Sedang mengirim seluruh data jawaban siswa ke Supabase...");

        float nilaiAkhir = ((float)totalJawabanBenar / activeQuestions.Count) * 100f;

        string attemptUrl = $"{supabaseUrl}/rest/v1/quiz_attempts";
        QuizAttempt attempt = new QuizAttempt { student_id = currentStudentId, meeting_id = selectedMeetingId, score = nilaiAkhir };
        string jsonAttempt = JsonUtility.ToJson(attempt);

        using (UnityWebRequest request = new UnityWebRequest(attemptUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonAttempt);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Prefer", "return=representation");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string res = request.downloadHandler.text.TrimStart('[').TrimEnd(']');
                QuizAttemptResponse savedAttempt = JsonUtility.FromJson<QuizAttemptResponse>(res);
                int generatedAttemptId = savedAttempt.id;

                yield return StartCoroutine(PostDetailedAnswers(generatedAttemptId));
                
                Debug.Log("<color=green>[SUCCESS]</color> Mantap! Data nilai & rincian jawaban siswa otomatis aman di PostgreSQL Supabase!");
            }
            else
            {
                Debug.LogError($"Gagal mengirim hasil kuis: {request.error}");
            }
        }
    }

    IEnumerator PostDetailedAnswers(int attemptId)
    {
        string answersUrl = $"{supabaseUrl}/rest/v1/answers";
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < activeQuestions.Count; i++)
        {
            if (userSelectedChoices.ContainsKey(i))
            {
                StudentAnswer ans = new StudentAnswer {
                    attempt_id = attemptId,
                    question_id = activeQuestions[i].id,
                    choice_id = userSelectedChoices[i].id
                };
                sb.Append(JsonUtility.ToJson(ans));
                if (i < activeQuestions.Count - 1) sb.Append(",");
            }
        }
        sb.Append("]");

        using (UnityWebRequest request = new UnityWebRequest(answersUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(sb.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
        }
    }
}