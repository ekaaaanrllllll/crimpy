using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuizSupabase; // Menggunakan data model yang kamu kirim

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
    public TMP_Text[] textChoices; 
    public Button[] buttonChoices; 

    [Header("Pengaturan Efek Jawaban")]
    public Color warnaBenar = Color.green;
    public Color warnaSalah = Color.red;
    public Vector2 ketebalanOutline = new Vector2(5f, -5f);
    public float durasiJedaEfek = 1.5f;

    [Header("UI Score References")]
    [Tooltip("Tarik objek Panel_Score dari Hierarchy ke sini")]
    public GameObject panelHasilSkor; 
    [Tooltip("Tarik teks untuk menampilkan nilai angka akhir (Tempat tulisan New Text)")]
    public TMP_Text textSkorAkhir;

    // State internal kuis
    private int currentStudentId;
    private int selectedMeetingId; 
    private List<Question> activeQuestions = new List<Question>();
    private List<Choice> activeChoices = new List<Choice>();
    
    private int currentQuestionIndex = 0;
    private int totalJawabanBenar = 0;
    private Dictionary<int, Choice> userSelectedChoices = new Dictionary<int, Choice>();
    
    private List<Outline> choiceOutlines = new List<Outline>();
    private bool sedangProsesPindahSoal = false;

    void Awake()
    {
        foreach (Button btn in buttonChoices)
        {
            if (btn != null)
            {
                Outline outline = btn.GetComponent<Outline>();
                if (outline == null) outline = btn.gameObject.AddComponent<Outline>();
                outline.effectDistance = ketebalanOutline;
                outline.enabled = false;
                choiceOutlines.Add(outline);
            }
        }
    }

    void OnEnable()
    {
        panelLogin.SetActive(true);
        panelGameKuis.SetActive(false);
        if (panelHasilSkor != null) panelHasilSkor.SetActive(false); 

        currentQuestionIndex = 0;
        totalJawabanBenar = 0;
        userSelectedChoices.Clear();
        sedangProsesPindahSoal = false;
        ResetEfekTombol();
    }

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

                // Memastikan respons tidak kosong dan bukan array kosong "[]"
                if (!string.IsNullOrEmpty(checkResponse) && checkResponse != "[]")
                {
                    // FIX PARSE ERROR: Karena Supabase mengembalikan Array [ {} ], kita bersihkan kurung sikunya terlebih dahulu
                    string cleanJson = checkResponse.TrimStart('[').TrimEnd(']');
                    
                    // Menggunakan StudentResponse karena data dari Supabase membawa field "id"
                    StudentResponse existingStudent = JsonUtility.FromJson<StudentResponse>(cleanJson);

                    currentStudentId = existingStudent.id;
                    Debug.Log($"<color=cyan>[SUPABASE INFO]</color> Siswa lama ditemukan! ID: {currentStudentId}");
                    
                    MasukKeGameKuis();
                    yield break; 
                }
            }
        }

        // JIKA SISWA BELUM TERDAFTAR, LANJUT DAFTAR BARU
        Debug.Log("[SUPABASE] Mendaftarkan sebagai siswa baru...");
        string postUrl = $"{supabaseUrl}/rest/v1/students";
        
        // FIX CONFLICT ERROR: Menggunakan class 'Student' (yang HANYA berisi name & class_name tanpa id)
        Student newStudent = new Student { name = nama, class_name = kelas };
        string jsonPayload = JsonUtility.ToJson(newStudent);

        using (UnityWebRequest request = new UnityWebRequest(postUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            
            // HANYA PAKAI YANG INI:
            request.downloadHandler = new DownloadHandlerBuffer();
            
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", $"Bearer {supabaseApiKey}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Prefer", "return=representation"); 

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // FIX PARSE ERROR: Bersihkan kurung siku array hasil respons POST sebelum di-parse
                string responseJson = request.downloadHandler.text.TrimStart('[').TrimEnd(']');
                StudentResponse savedStudent = JsonUtility.FromJson<StudentResponse>(responseJson);
                
                currentStudentId = savedStudent.id; // Sukses mendapatkan ID otomatis dari database (1, 2, 3...)
                Debug.Log($"<color=green>[SUPABASE SUCCESS]</color> ID Baru: {currentStudentId}");

                MasukKeGameKuis();
            }
            else
            {
                Debug.LogError($"Gagal mendaftarkan siswa: {request.downloadHandler.text}");
            }
        }
    }

    void MasukKeGameKuis()
    {
        panelLogin.SetActive(false);
        panelGameKuis.SetActive(true);
        StartCoroutine(GetMeetingIdThenFetchQuestions());
    }

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

    void TampilkanSoalKeUI()
    {
        if (activeQuestions.Count == 0)
        {
            textPertanyaan.text = "Tidak ada soal untuk pertemuan ini di database, brow.";
            return;
        }

        ResetEfekTombol();
        sedangProsesPindahSoal = false;

        Question currentQuestion = activeQuestions[currentQuestionIndex];
        textPertanyaan.text = currentQuestion.question_text;

        List<Choice> filteredChoices = activeChoices.FindAll(c => c.question_id == currentQuestion.id);

        for (int i = 0; i < 4; i++)
        {
            if (i < filteredChoices.Count)
            {
                buttonChoices[i].gameObject.SetActive(true);
                buttonChoices[i].interactable = true; 
                Choice choiceData = filteredChoices[i];
                textChoices[i].text = $"{choiceData.choice_label}.\n{choiceData.choice_text}";
                
                int indexTombol = i; 
                buttonChoices[i].onClick.RemoveAllListeners();
                buttonChoices[i].onClick.AddListener(() => JawabSoalWithFeedback(choiceData, indexTombol));
            }
            else
            {
                buttonChoices[i].gameObject.SetActive(false);
            }
        }
    }

    void JawabSoalWithFeedback(Choice pilihanYangDipilih, int indexTombol)
    {
        if (sedangProsesPindahSoal) return;
        sedangProsesPindahSoal = true;

        foreach (Button btn in buttonChoices) btn.interactable = false;

        userSelectedChoices[currentQuestionIndex] = pilihanYangDipilih;

        if (pilihanYangDipilih.is_correct)
        {
            totalJawabanBenar++;
            choiceOutlines[indexTombol].effectColor = warnaBenar;
            choiceOutlines[indexTombol].enabled = true;
        }
        else
        {
            choiceOutlines[indexTombol].effectColor = warnaSalah;
            choiceOutlines[indexTombol].enabled = true;

            List<Choice> filteredChoices = activeChoices.FindAll(c => c.question_id == activeQuestions[currentQuestionIndex].id);
            for (int i = 0; i < filteredChoices.Count; i++)
            {
                if (filteredChoices[i].is_correct && i < choiceOutlines.Count)
                {
                    choiceOutlines[i].effectColor = warnaBenar;
                    choiceOutlines[i].enabled = true;
                }
            }
        }

        StartCoroutine(JedaPindahSoal());
    }

    IEnumerator JedaPindahSoal()
    {
        yield return new WaitForSeconds(durasiJedaEfek);

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

    void ResetEfekTombol()
    {
        foreach (Outline outline in choiceOutlines)
        {
            if (outline != null) outline.enabled = false;
        }
    }

    IEnumerator PostQuizAttemptAndAnswers()
    {
        panelGameKuis.SetActive(false); 
        Debug.Log("[PROCESS] Soal habis! Mengirim hasil kuis siswa...");

        float nilaiAkhir = ((float)totalJawabanBenar / activeQuestions.Count) * 100f;
        int nilaiBulat = Mathf.RoundToInt(nilaiAkhir); 

        string attemptUrl = $"{supabaseUrl}/rest/v1/quiz_attempts";
        
        // Menggunakan class QuizAttempt (Tanpa field ID agar auto-increment)
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
                // FIX PARSE ERROR: Bersihkan [] pada response quiz_attempts sebelum di-parse ke QuizAttemptResponse
                string res = request.downloadHandler.text.TrimStart('[').TrimEnd(']');
                QuizAttemptResponse savedAttempt = JsonUtility.FromJson<QuizAttemptResponse>(res);
                int generatedAttemptId = savedAttempt.id;

                yield return StartCoroutine(PostDetailedAnswers(generatedAttemptId));
                
                Debug.Log("<color=green>[SUCCESS]</color> Seluruh data tersimpan!");
                BukaHalamanSkor(nilaiBulat);
            }
            else
            {
                Debug.LogError($"Gagal mengirim hasil kuis: {request.error}");
                BukaHalamanSkor(nilaiBulat);
            }
        }
    }

    void BukaHalamanSkor(int nilai)
    {
        if (panelHasilSkor != null)
        {
            panelHasilSkor.SetActive(true); // Membuka Panel_Score otomatis

            if (textSkorAkhir != null)
            {
                textSkorAkhir.text = nilai.ToString(); // Mengubah teks "New Text" jadi nilai murni angka
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