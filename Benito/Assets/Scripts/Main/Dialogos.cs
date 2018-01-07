using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dialogos : MonoBehaviour {

    public AudioClip dialogFx;
    public int maxNumberOfCharacters = 10;
    public float characterSpeed = 0.5f; // Velocidad para cambiar de caracter

    // Array para almacenar los indices de los dialogos
    public Dialogs[] dialogs;

    // Clase para almacenar los dialogos //
    [System.Serializable]
    public class Dialogs
    {
        [TextArea]
        public string[] dialog;
    }


    [HideInInspector]
    public bool onDialog = false;   // Flag para indicar que esta en un dialogo

    private bool dialogEnded = false;   // Flag para indicar que se ha llegado al final del dialogo
    private int dialogIndex = 0, pageIndex = 0;    // Indice del dialogo actual
    private int characterIndex = 0; // Indice del caracter en el string del dialogo actual
    private int firstCharacterIndex = 0;    // Indice del primer caracter (util para cuando no ha cabido el texto en la caja)
    private float characterTimer = 0.0f;    // Temporizador para cambiar de caracter

    private Text dialogText; // Caja de texto para los dialogos

    private bool dontFit = false;   // Flag para indicar que el texto no se ha completado

    private int characterMultiplier = 1;    // Valor por el que se multiplica el numero maximo de caracteres que caben en la caja

    private bool ended = false; // Flag para controlar cuando se ha acabado el texto en la pagina

    private int _characterIndex = 0;    // Valor para controlar cuando el indice del caracter se ha salido del total de caracteres de la pagina

    private float _speed;   // Valor para controlar la velocidad del texto de forma dinamica

    private GameObject benitoDialogObj, saraDialogObj, directorDialogObj;   // Objetos de dialogo de cada personaje

    private AudioSource audioSource;    // Audio

    private bool activated = false;

	// Use this for initialization
	void Start () {
        // Valores iniciales
        activated = false;
        SetUp();
        dialogEnded = true;
        dialogIndex = 0;
        pageIndex = 0;
        characterIndex = 0;
        characterTimer = 0.0f;
        firstCharacterIndex = 1;
        dialogText.transform.parent.gameObject.SetActive(false);
        _speed = characterSpeed;
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        SetUp();
        ManageDialog();
	}

    void SetUp()
    {
        if (!activated)
        {
            // Busca el objeto del texto
            GameObject dialogObj = GameObject.FindGameObjectWithTag("DialogTxt");
            dialogText = dialogObj.transform.GetChild(dialogObj.transform.childCount - 1).GetComponent<Text>();
            // Añade la funcion en click al boton del texto
            Button btn = dialogText.gameObject.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ChangeDialog);
            // Busca y enlaza los objetos de dialogo
            for (int i = 0; i < dialogObj.transform.childCount; i++)
            {
                switch (dialogObj.transform.GetChild(i).gameObject.name)
                {
                    case "Benito":
                        benitoDialogObj = dialogObj.transform.GetChild(i).gameObject;
                        break;
                    case "Sara":
                        saraDialogObj = dialogObj.transform.GetChild(i).gameObject;
                        break;
                    case "Director":
                        directorDialogObj = dialogObj.transform.GetChild(i).gameObject;
                        break;
                }
            }
            if (SceneManager.GetActiveScene().name == "TuberíasLocas" && PlayerPrefs.GetInt("TuberiasTutorial") != 1)
            {
                PlayDialog(1);
                PlayerPrefs.SetInt("TuberiasTutorial", 1);
            }
            else if (SceneManager.GetActiveScene().name == "Humedades" && PlayerPrefs.GetInt("HumedadesTutorial") != 1)
            {
                PlayDialog(2);
                PlayerPrefs.SetInt("HumedadesTutorial", 1);
            }
            else if (SceneManager.GetActiveScene().name == "PenDrives" && PlayerPrefs.GetInt("PenDrivesTutorial") != 1)
            {
                PlayDialog(3);
                PlayerPrefs.SetInt("PenDrivesTutorial", 1);
            }
            else
            {
                dialogObj.SetActive(false);
            }

            activated = true;
        }
    }
    void ManageDialog()
    {
        if (!dialogEnded)
        {
            //bool benitoTalks = false, saraTalks = false, directorTalks = false;
            foreach (string page in dialogs[dialogIndex].dialog)
            {
                switch (page.ToCharArray().GetValue(0).ToString())
                {
                    case "d":
                        //directorTalks = true;
                        directorDialogObj.SetActive(true);
                        break;
                    case "b":
                        //benitoTalks = true;
                        benitoDialogObj.SetActive(true);
                        break;
                    case "s":
                        //saraTalks = true;
                        saraDialogObj.SetActive(true);
                        break;
                }
                
            }
            // Comprueba quien esta hablando y muestralos debidamente
            Color t = Color.white;
            t.a = 0.4f;
            Color o = Color.white;
            switch (dialogs[dialogIndex].dialog[pageIndex].ToCharArray().GetValue(0).ToString())
            {
                case "d":
                    directorDialogObj.GetComponent<Image>().color = o;
                    saraDialogObj.GetComponent<Image>().color = t;
                    benitoDialogObj.GetComponent<Image>().color = t;
                    break;
                case "b":
                    directorDialogObj.GetComponent<Image>().color = t;
                    saraDialogObj.GetComponent<Image>().color = t;
                    benitoDialogObj.GetComponent<Image>().color = o;
                    break;
                case "s":
                    directorDialogObj.GetComponent<Image>().color = t;
                    saraDialogObj.GetComponent<Image>().color = o;
                    benitoDialogObj.GetComponent<Image>().color = t;
                    break;
            }
            // Marca que esta en dialogo
            onDialog = true;
            // Activa la caja de dialogo
            dialogText.transform.parent.gameObject.SetActive(true);
            // Saca los caracteres por pantalla
            dialogText.text = dialogs[dialogIndex].dialog[pageIndex].Substring(firstCharacterIndex, characterIndex);
            // Temporizador
            characterTimer += Time.deltaTime;
            if (characterTimer >= _speed)
            {
                // Limite alcanzado
                if (_characterIndex + 1 == dialogs[dialogIndex].dialog[pageIndex].Length)
                {
                    ended = true;
                }
                else if (characterIndex >= maxNumberOfCharacters && !ended)
                {
                    dontFit = true;
                }
                // Cambia de caracter
                else
                {
                    characterTimer = 0;
                    characterIndex++;
                    _characterIndex++;
                    audioSource.clip = dialogFx;
                    audioSource.Play();
                }
            }
        }
        else
        {
            onDialog = false;
        }
    }

    void ChangeDialog()
    {
        // Si ha acabado...
        if (ended)
        {
            // Comprueba si hay mas paginas para el dialogo
            if (dialogs[dialogIndex].dialog.Length > pageIndex + 1)
            {
                pageIndex++;
                characterIndex = 0;
                firstCharacterIndex = 1;
                characterMultiplier = 1;
                characterTimer = 0;
                _characterIndex = 0;
                _speed = characterSpeed;
                dontFit = false;
                ended = false;
            }
            // Si no, comprueba si hay mas dialogos
            /*else if (dialogs.Length > dialogIndex + 1)
            {
                dialogIndex++;
                pageIndex = 0;
                characterIndex = 0;
                firstCharacterIndex = 0;
                characterMultiplier = 1;
                characterTimer = 0;
                _characterIndex = 0;
                _speed = characterSpeed;
                dontFit = false;
                ended = false;
            }*/
            // Si no, termina el dialogo
            else
            {
                pageIndex = 0;
                dialogIndex = 0;
                characterIndex = 0;
                firstCharacterIndex = 1;
                characterMultiplier = 1;
                characterTimer = 0;
                _characterIndex = 0;
                _speed = characterSpeed;
                dialogText.transform.parent.gameObject.SetActive(false);
                dontFit = false;
                dialogEnded = true;
                ended = false;
            }
        }
        // Si el texto no ha cabido...
        else if (dontFit)
        {
            firstCharacterIndex = (characterIndex * characterMultiplier) + 1;
            dontFit = false;
            characterMultiplier++;
            characterIndex = 0;
            characterTimer = 0;
            _speed = characterSpeed;
            return;
        }
        // Si ha clickado pero no ha acabado...
        else
        {
            _speed -= characterSpeed / 2f;  // Acelera la velocidad
        }
    }

    /* Metodo para reproducir un dialogo */
    public void PlayDialog(int index)
    {
        dialogIndex = index;
        dialogEnded = false;
    }

    /* Al activarse */
    private void OnEnable()
    {
        activated = false;
        
    }
}
