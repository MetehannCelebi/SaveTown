using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameKontrolcu : MonoBehaviour
{
    int aktifsira;
    float health = 100;

    [Header("SAĞLIK AYARLARI")]
    public Image HealthBar;

    [Header("SİLAH AYARLARI")]
    public GameObject[] silahlar;
    public AudioSource degisimSesi;
    public GameObject Bomba;
    public GameObject BombaPoint;
    public Camera benimCam;

    [Header("DUSMAN AYARLARI")]
    public GameObject[] dusmanlar;
    public GameObject[] cikisNoktalari;
    public GameObject[] hedefNoktalar;
    public float DusmancikmaSuresi;
    public TextMeshProUGUI KalanDusman_text;
    public int Baslangic_dusman_sayisi;
    public static int Kalan_dusman_sayisi;

    [Header("DİĞER AYARLAR")]
    public GameObject GameOverCanvas;
    public GameObject KazandinCanvas;
    public GameObject PauseCanvas;
    public AudioSource OyunIcSes;
    public TextMeshProUGUI Saglik_sayisi_text;
    public TextMeshProUGUI Bomba_sayisi_text;
    public AudioSource ItemYok;

    public static bool OyunDurdumu;


    void Start()
    {
        Baslangicİslemleri();       

    }
    IEnumerator DusmanCikar()
    {

        while (true)
        {
            yield return new WaitForSeconds(DusmancikmaSuresi);

            if (Baslangic_dusman_sayisi != 0)
            {
                int dusman = Random.Range(0, 5);
                int cikisnoktasi = Random.Range(0, 2);
                int hedefnoktasi = Random.Range(0, 2);

                GameObject Obje = Instantiate(dusmanlar[dusman], cikisNoktalari[cikisnoktasi].transform.position, Quaternion.identity);
                Obje.GetComponent<Dusman>().HedefBelirle(hedefNoktalar[hedefnoktasi]);
                Baslangic_dusman_sayisi--;
            }


        }

    }

    void Baslangicİslemleri()
    {
        OyunDurdumu = false;
        PlayerPrefs.SetInt("Taramali_Mermi", 70);
        PlayerPrefs.SetInt("Pompali_Mermi", 50);
        PlayerPrefs.SetInt("Magnum_Mermi", 30);
        PlayerPrefs.SetInt("Sniper_Mermi", 20);
        PlayerPrefs.SetInt("Saglik_sayisi", 1);
        PlayerPrefs.SetInt("Bomba_sayisi", 5);
        PlayerPrefs.SetInt("OyunBasladimi", 1);
        if (!PlayerPrefs.HasKey("OyunBasladimi"))
        {
            PlayerPrefs.SetInt("Taramali_Mermi", 70);
            PlayerPrefs.SetInt("Pompali_Mermi", 50);
            PlayerPrefs.SetInt("Magnum_Mermi", 30);
            PlayerPrefs.SetInt("Sniper_Mermi", 20);
            PlayerPrefs.SetInt("Saglik_sayisi", 1);
            PlayerPrefs.SetInt("Bomba_sayisi", 5);
            PlayerPrefs.SetInt("OyunBasladimi", 1);

        }
        KalanDusman_text.text = Baslangic_dusman_sayisi.ToString();
        Kalan_dusman_sayisi = Baslangic_dusman_sayisi;

        Saglik_sayisi_text.text = PlayerPrefs.GetInt("Saglik_sayisi").ToString();
        Bomba_sayisi_text.text = PlayerPrefs.GetInt("Bomba_sayisi").ToString();
        
        aktifsira = 0;

        OyunIcSes = GetComponent<AudioSource>();
        OyunIcSes.Play();


        StartCoroutine(DusmanCikar());
       

    }
   public void Dusman_sayisi_guncelle()
    {
        Kalan_dusman_sayisi--;

        if (Kalan_dusman_sayisi<=0)
        {
            KazandinCanvas.SetActive(true);
            KalanDusman_text.text = "0";
            Time.timeScale = 0;
        }
        else
        {
            KalanDusman_text.text = Kalan_dusman_sayisi.ToString();
        }
       
    }
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Alpha1) && !OyunDurdumu)
        {
            Silahdegistir(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !OyunDurdumu)
        {
            Silahdegistir(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && !OyunDurdumu)
        {
            Silahdegistir(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && !OyunDurdumu)
        {
            Silahdegistir(3);
        }
        if (Input.GetKeyDown(KeyCode.Q) && !OyunDurdumu)
        {
            QTusuVersiyonuSilahdegistir();

        }
        if (Input.GetKeyDown(KeyCode.G) && !OyunDurdumu)
        {
            BombaAt();
        }
        if (Input.GetKeyDown(KeyCode.E) && !OyunDurdumu)
        {
            Saglik_doldur();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && !OyunDurdumu)
        {
            Pause();
        }

    }

    public void DarbeAl(float darbegucu)
    {
        health -= darbegucu;
        HealthBar.fillAmount = health / 100;
        if (health <= 0)
        {
            GameOver();

        }

    }
    public void Saglik_doldur()
    {


        if (PlayerPrefs.GetInt("Saglik_sayisi")!=0 && health!=100)
        {
            health = 100;
            HealthBar.fillAmount = health / 100;
            PlayerPrefs.SetInt("Saglik_sayisi", PlayerPrefs.GetInt("Saglik_sayisi") - 1);
            Saglik_sayisi_text.text = PlayerPrefs.GetInt("Saglik_sayisi").ToString();
        }
        else
        {

            ItemYok.Play();
        }

        
    }
    public void Saglik_Al()
    {       
        PlayerPrefs.SetInt("Saglik_sayisi", PlayerPrefs.GetInt("Saglik_sayisi") + 1);
        Saglik_sayisi_text.text = PlayerPrefs.GetInt("Saglik_sayisi").ToString();
    }
    void BombaAt()
    {

        if (PlayerPrefs.GetInt("Bomba_sayisi") != 0)
        {
            GameObject obje = Instantiate(Bomba, BombaPoint.transform.position, BombaPoint.transform.rotation);
            Rigidbody rg = obje.GetComponent<Rigidbody>();
            Vector3 acimiz = Quaternion.AngleAxis(90, benimCam.transform.forward) * benimCam.transform.forward;
            rg.AddForce(acimiz * 250f);

            PlayerPrefs.SetInt("Bomba_sayisi", PlayerPrefs.GetInt("Bomba_sayisi") - 1);
            Bomba_sayisi_text.text = PlayerPrefs.GetInt("Bomba_sayisi").ToString();
        }
        else
        {

            ItemYok.Play();
        }

       

    }
    public void Bomba_Al()
    {
        PlayerPrefs.SetInt("Bomba_sayisi", PlayerPrefs.GetInt("Bomba_sayisi") + 1);
        Bomba_sayisi_text.text = PlayerPrefs.GetInt("Bomba_sayisi").ToString();
    }
    void GameOver()
    {

        GameOverCanvas.SetActive(true);
        Time.timeScale = 0;
        OyunDurdumu = true;
        Cursor.visible = true;
        GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;
       
            
    }
    void Silahdegistir(int siraNumarasi)
    {
        degisimSesi.Play();
        foreach (GameObject silah in silahlar)
        {
            silah.SetActive(false);

        }
        aktifsira = siraNumarasi;
        silahlar[siraNumarasi].SetActive(true);

    }
    void QTusuVersiyonuSilahdegistir()
    {

        degisimSesi.Play();

        foreach (GameObject silah in silahlar)
        {
            silah.SetActive(false);
        }
        if (aktifsira == 3)
        {
            aktifsira = 0;
            silahlar[aktifsira].SetActive(true);

        }
        else
        {
            aktifsira++;
            silahlar[aktifsira].SetActive(true);

        }


    }
    public void BastanBasla()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        OyunDurdumu = false;
        Cursor.visible = false;
        GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Pause()
    {
        PauseCanvas.SetActive(true);
        Time.timeScale = 0;
        OyunDurdumu = true;
        Cursor.visible = true;
        GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = false;
        Cursor.lockState = CursorLockMode.None;

    }
    public void DevamEt()
    {
        PauseCanvas.SetActive(false);
        Time.timeScale = 1;
        OyunDurdumu = false;
        Cursor.visible = false;
        GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().m_MouseLook.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;

    }
    public void anaMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
