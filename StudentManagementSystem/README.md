# 🎓 Student Management System (C# - .NET 9)

Kapsamlı bir öğrenci yönetim sistemi. Rol bazlı giriş, ödev yönetimi, notlandırma ve sınıf-ders organizasyonu gibi modülleri içerir. .NET 9 teknolojisi ile geliştirilmiş, Entity Framework Core kullanılarak inşa edilmiştir.

---

## 🧰 Kullanılan Teknolojiler

| Katman/Modül         | Teknoloji            |
|----------------------|----------------------|
| Backend              | C# (.NET 9)          |
| ORM                  | Entity Framework Core|
| UI                   | Console (CLI UX)     |
| Veritabanı           | SQLite / SQL Server  |
| Kimlik Doğrulama     | Hash + Temp Password |
| Mimari               | Katmanlı Mimari      |

---

## 📚 Özellikler

### 👥 Kullanıcı Girişi ve Rolleri
- Admin, Öğretmen ve Öğrenci rolleri
- Giriş doğrulaması (TCKN + Şifre)
- Geçici şifre ve ilk girişte zorunlu değişim

### 🏫 Sınıf Yönetimi
- Sınıf oluşturma, düzenleme, silme, listeleme
- Öğrenci/öğretmen çoklu sınıf ilişkisi

### 📖 Ders Yönetimi
- Ders ekleme/silme/güncelleme
- Öğretmene ve sınıfa ders atama

### 📝 Ödev Yönetimi
- Öğretmen tarafından ödev verme
- Öğrencilerin teslim süreci
- Teslim tarihine göre sıralama

### 🧮 Not Sistemi
- Öğrencilere not verme ve geri bildirim
- Not güncelleme/silme işlemleri
- Öğrenci panelinde not görüntüleme
---

> İlk giriş için admin hesabı:  
> **TCKN:** 12345678912  
> **Şifre:** admin

---

## 🧭 İş Akışı ve Veri İlişkileri

### 👨‍🏫 Öğretmenin Derse ve Sınıfa Atanması

- Öğretmen kaydı sırasında:
  - Sistem, öğretmeni oluşturur (`User + Teacher`).
  - Bir veya birden fazla sınıfla eşleştirme yapılır (`Teacher ⇄ Classroom`).
- Yönetici ya da öğretmen, ders ataması yapabilir (`Teacher ⇄ Lesson`).
- Her ders birden fazla sınıfa atanabilir (`Lesson ⇄ Classroom`).

> **Not:** Bir öğretmen birden fazla derse, ders birden fazla sınıfa atanabilir.

---

### 🧑‍🎓 Öğrencinin Ödev Alması

- Öğrenci bir veya birden fazla sınıfa kayıtlı olabilir (`Student ⇄ Classroom`).
- Dersler sınıflara atanır, böylece öğrenciler dolaylı olarak ders almış olur.
- Öğretmen, dersi üzerinden bir ödev tanımlar (`Lesson → Homework`).
- Ödevin ait olduğu ders, sınıflara atanmış olduğundan o sınıftaki her öğrenci o ödevi **otomatik olarak alır**.

---

### 📤 Ödev Teslim Süreci

- Öğrenci panelinden, atanmış ödevler listelenir.
- Öğrenci, belirli bir ödevi **"Teslim Et"** işlemiyle teslim eder.
- Bu işlem:
  - `Grade` tablosuna teslim bilgisi kaydeder.
  - `IsSubmitted = true` ve `SubmittedAt = DateTime.Now` olarak işaretlenir.

---

### 💾 Not Verme ve Geri Bildirim

- Öğretmen panelinde "Not Ver" menüsünden:
  - Kendi verdiği ödevler listelenir.
  - O ödevin ilişkili olduğu sınıflardaki öğrenciler listelenir.
- Öğretmen her öğrenciye:
  - Not (`Score`)
  - Yorum/geri bildirim (`Feedback`)
  verebilir.

---

### 📊 Teslim Durumları ve Görselleştirme

- Öğrenciler, her ödev için:
  - Teslim Edildi mi?
  - Teslim Tarihi
  - Verilen Not
  - Öğretmen Yorumları
  gibi bilgileri görebilir.

---
