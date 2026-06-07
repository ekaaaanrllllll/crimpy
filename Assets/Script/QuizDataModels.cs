using System;
using System.Collections.Generic;

namespace QuizSupabase
{
    // ==========================================
    // DATA MODEL: MEETINGS
    // ==========================================
    [Serializable]
    public class Meeting
    {
        public int id;
        public int meeting_number;
        public string title;
    }
    [Serializable] public class MeetingList { public List<Meeting> data; }

    // ==========================================
    // DATA MODEL: QUESTIONS
    // ==========================================
    [Serializable]
    public class Question
    {
        public int id;
        public int meeting_id;
        public string question_text;
        public int question_order;
    }
    [Serializable] public class QuestionList { public List<Question> data; }

    // ==========================================
    // DATA MODEL: CHOICES
    // ==========================================
    [Serializable]
    public class Choice
    {
        public int id;
        public int question_id;
        public string choice_label;
        public string choice_text;
        public bool is_correct;
    }
    [Serializable] public class ChoiceList { public List<Choice> data; }

    // ==========================================
    // DATA MODEL: STUDENTS
    // ==========================================
    // Ditulis terpisah agar Unity TIDAK MENGIRIM field "id" ke Supabase saat daftar
    [Serializable]
    public class Student
    {
        public string name;
        public string class_name;
    }

    // Class ini khusus dipakai saat MENERIMA respon balik pendaftaran dari Supabase
    [Serializable]
    public class StudentResponse
    {
        public int id; 
        public string name;
        public string class_name;
    }

    // ==========================================
    // DATA MODEL: QUIZ ATTEMPTS (HASIL KUIS)
    // ==========================================
    // Dipakai untuk MENGIRIM data hasil kuis (ID dihapus agar digenerate otomatis oleh database)
    [Serializable]
    public class QuizAttempt
    {
        public int student_id;
        public int meeting_id;
        public float score;
    }

    // Dipakai khusus untuk MENERIMA respon balik dari tabel quiz_attempts setelah sukses dikirim
    [Serializable]
    public class QuizAttemptResponse
    {
        public int id;
        public int student_id;
        public int meeting_id;
        public float score;
    }

    // ==========================================
    // DATA MODEL: ANSWERS (RINCIAN JAWABAN PER NOMOR)
    // ==========================================
    // Variabel id dibuang agar database Supabase mengisi auto-increment id tersendiri
    [Serializable]
    public class StudentAnswer
    {
        public int attempt_id;
        public int question_id;
        public int choice_id;
    }
}