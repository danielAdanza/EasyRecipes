using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using ChartboostSDK;

public struct Recipe
{
    public string Title { get; set; }
    public int CantTitle { get; set; }
    public string CantString { get; set; }
    public string Foto { get; set; }
    public int[] IngredientCant { get; set; }
    public string[] IngredientDesc { get; set; }
    public int Difficulty { get; set; }
    public int PreparationTime { get; set; }
    public string Category { get; set; }
    public bool Lactosa { get; set; }
    public bool Gluten { get; set; }
    public string[] Steps { get; set; }

    public Recipe(string title, int cantTitle, string cantString, string foto, int[] ingredientCant, string[] ingredientDesc, int difficulty, int preparationTime, string category, bool lactosa, bool gluten, string[] steps)
    {
        Title = title;
        CantTitle = cantTitle;
        CantString = cantString;
        Foto = foto;
        IngredientCant = ingredientCant;
        IngredientDesc = ingredientDesc;
        Difficulty = difficulty;
        PreparationTime = preparationTime;
        Category = category;
        Lactosa = lactosa;
        Gluten = gluten;
        Steps = steps;
        
    }
}

public class gameData : MonoBehaviour
{
    //recipe object
    private Recipe recipe;
    private int categoryRecipe = 1;
    private int numberRecipe = 1;
    private int languajeRecipe = 1;
    private int stepsRecipe = 0;
    private float portions = 1.0f;     //the chosen protions in the UI
    //UI objects
    //menu
    public Text ingredientsText;
    public Text pasosText;
    //sections
    public GameObject infoSection;
    public GameObject ingredientsSection;
    public GameObject stepsSection;
    public GameObject menuArrows;
    public GameObject arrowLeft;
    public GameObject arrowRight;
    private int section = 0;
    //UI inside Info Section
    public Text timeTitle;
    public Text timeText;
    public Text difficultyTitle;
    public Text difficultyText;
    public Text containsText;
    public Text categoryTitle;
    public Text categoryText;
    public Text titleText;
    public RawImage recipeImage;
    public Texture [] recipeTextures;
    //UI for ingredients
    public GameObject ingredientObject;
    public GameObject ingredientList;
    public Text buttonQuantity1;
    public Text buttonQuantity2;
    public Text buttonQuantity3;
    public Text ingredientsTitle;
    //UI for steps
    public Text stepsText;
    public Text stepsTitle;

    public AudioSource audio;

    // Use this for initialization
    void Start ()
    {
        //playing the audio in order to look like a book
        if (UnityEngine.PlayerPrefs.GetInt("sound") != 1)
        {
            audio.Play();
        }

        //showing the ads from Chartboost
        Chartboost.showInterstitial(CBLocation.Default);

        //Category Recipe:
        //CAT 1: dessert
        //CAT 2: appetizer
        //CAT 3: diet
        //CAT 4: thermomix
        //CAT 5: drinks
        //CAT 6: meats

        //Languaje Recipe:
        //LAN 1: spanish
        //LAN 2: english

        //loading data from playerPrefs
        categoryRecipe = UnityEngine.PlayerPrefs.GetInt("category");
        languajeRecipe = UnityEngine.PlayerPrefs.GetInt("languaje");
        numberRecipe   = UnityEngine.PlayerPrefs.GetInt("recipeNumber");

        switch (categoryRecipe)
        {
            case 1:
                if (languajeRecipe == 1)
                {   ChooseDessertSP(numberRecipe);  }
                else
                {   ChooseDessertSP(numberRecipe);  }
                break;
            case 2:
                if (languajeRecipe == 1)
                { ChooseAppetizerSP(numberRecipe); }
                else
                { ChooseAppetizerEN(numberRecipe); }
                break;
            case 3:
                if (languajeRecipe == 1)
                { ChooseDietSP(numberRecipe); }
                else
                { ChooseDietEN(numberRecipe); }
                break;
            case 4:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseThermomixSP(numberRecipe); }
                else
                { ChooseThermomixEN(numberRecipe); }
                break;
            case 5:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseDrinksSP(numberRecipe); }
                else
                { ChooseDrinksEN(numberRecipe); }
                break;
            case 6:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseMeatsSP(numberRecipe); }
                else
                { ChooseMeatsEN(numberRecipe); }
                break;
        }

        recipeImage.texture = recipeTextures[Int32.Parse(recipe.Foto)];

        //it fills the information in the UI but only for the info section which is the first one in appearing
        if (languajeRecipe == 1)
        {
            FillInformationInfoSP();
        }
        else
        {
            FillInformationInfoEN();

            //changing the text of the buttons into English
            ingredientsText.text = "Ingredients";
            pasosText.text = "Steps";
        }

        FillSteps(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBackToIndex();
        }
    }
    //it changes the quantity in the ingredients section
    public void ChangeSomething (int i)
    {
        float j = (float) i / 2;
        portions = j;

        FillIngredientsSPEN();
    }

    public void GoBackToIndex ()
    {
        SceneManager.LoadScene("indexMenu");
    }

    //It changes section depending on which button you pressed. It will also go to the next step
    public void ChangeSection (int newSection)
    {
        infoSection.gameObject.SetActive(false);
        ingredientsSection.gameObject.SetActive(false);
        stepsSection.gameObject.SetActive(false);
        menuArrows.gameObject.SetActive(false);

        switch (newSection)
        {
            case 1:
                infoSection.gameObject.SetActive(true);
                break;
            case 2:
                ingredientsSection.gameObject.SetActive(true);
                FillIngredientsSPEN();
                break;
            case 3:
                stepsSection.gameObject.SetActive(true);
                menuArrows.gameObject.SetActive(true);

                //it will lock the the forward button if it is the last element of the array
                if ((stepsRecipe + 1) == recipe.Steps.Length)
                {
                    arrowRight.gameObject.SetActive(false);
                }
                else if (stepsRecipe == 0)
                {
                    arrowLeft.gameObject.SetActive(false);
                }

                break;
        }
    }
    //this function will fill the text information related with the recipe that we want to show in the UI
    public void FillInformationInfoSP ()
    {
        timeText.text = ConvertMin(recipe.PreparationTime);
        difficultyText.text = recipe.Difficulty + " / 5";
        titleText.text = recipe.Title;

        if (recipe.Gluten == true && recipe.Lactosa == true)
        {
            containsText.text = " Gluten y Lactosa";
        }
        else if (recipe.Gluten == true)
        {
            containsText.text = "Gluten";
        }
        else if (recipe.Gluten == true)
        {
            containsText.text = "Lactosa";
        }
        else
        {
            containsText.text = "Sin Glutem y Lactosa";
        }

        categoryText.text = recipe.Category;
    }

    public void FillInformationInfoEN ()
    {
        timeTitle.text = "Time: ";
        difficultyTitle.text = "Difficulty: ";
        categoryTitle.text = "Category: ";
        timeText.text = ConvertMin(recipe.PreparationTime);
        difficultyText.text = recipe.Difficulty + " / 5";
        titleText.text = recipe.Title;

        if (recipe.Gluten == true && recipe.Lactosa == true)
        {
            containsText.text = " Gluten y Lactose";
        }
        else if (recipe.Gluten == true)
        {
            containsText.text = "Gluten";
        }
        else if (recipe.Gluten == true)
        {
            containsText.text = "Lactose";
        }
        else
        {
            containsText.text = "without Glutem & Lactose";
        }

        categoryText.text = recipe.Category;
    }

    //it converts the minutes number into a more understandable format inside an string
    public String ConvertMin (int num)
    {
        //is it less than one hour?
        if (num < 60)
        {
            return num + " Min.";
        }
        //is is more than an hour?
        else
        {
            int hours = num / 60;
            int min = num % 60;

            //do you have any minutes to show?
            if (min > 0)
            {
                return hours + " H. " + min + " Min.";
            }
            else
            {
                return hours + " H. ";
            }
        }
    }

    public void FillIngredientsSPEN ()
    {
        //an auxiliar variable used to calculate the quantity for the ingredients
        float portionNumber = 0.0f;
        //if the languaje is in English then we change a phrase in the ingredient Section
        if (languajeRecipe != 1)
        {
            ingredientsTitle.text = "Needed ingredients";
        }

        //filling the text for the buttons
        buttonQuantity1.text = (recipe.CantTitle / 2) + " " + recipe.CantString;
        buttonQuantity2.text = recipe.CantTitle + " " + recipe.CantString;
        buttonQuantity3.text = (recipe.CantTitle * 2) + " " + recipe.CantString;

        //creating the ingredient list
        for (int i = ingredientList.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(ingredientList.transform.GetChild(i).gameObject);
        }

        for (int i = 0 ; i < recipe.IngredientCant.Length ; i++ )
        {
            GameObject ingredientPanel;
            ingredientPanel = Instantiate(ingredientObject) as GameObject;
            ingredientPanel.transform.SetParent(ingredientList.transform);

            portionNumber = portions * recipe.IngredientCant[i];

            //if the portion is 0 then don't show the quantity cause we now that is an uncountable such as sal or olive oil
            if (portionNumber != 0)
            {
                ingredientPanel.transform.Find("ingrendientToggle").Find("Label").GetComponent<Text>().text = portionNumber + " " + recipe.IngredientDesc[i];
            }
            else
            {
                ingredientPanel.transform.Find("ingrendientToggle").Find("Label").GetComponent<Text>().text = recipe.IngredientDesc[i];
            }
            
            ingredientPanel.transform.Find("ingrendientToggle").GetComponent<Toggle>().isOn = false;
        }
        
    }

    //step contains +1 or -1 depending on the arrow
    public void FillSteps (int step)
    {
        stepsRecipe = stepsRecipe + step;

        //setting the buttons to active always
        arrowLeft.gameObject.SetActive(true);
        arrowRight.gameObject.SetActive(true);

        //if it is the first step
        if (stepsRecipe < 0)
        {
            stepsRecipe = 0;
        }
        //if it is the second step
        else if (stepsRecipe >= recipe.Steps.Length)
        {
            stepsRecipe--;
        }

        //it will lock the the forward button if it is the last element of the array
        if ( (stepsRecipe + 1) == recipe.Steps.Length)
        {
            arrowRight.gameObject.SetActive(false);
        }
        else if ( stepsRecipe == 0 )
        {
            arrowLeft.gameObject.SetActive(false);
        }

        //if the recipe only has one step it will hide both arrows
        if (recipe.Steps.Length <= 1)
        {
            arrowLeft.gameObject.SetActive(false);
        }

        stepsTitle.text = pasosText.text + " : " + (stepsRecipe + 1) + " / " + recipe.Steps.Length;

        stepsText.text = recipe.Steps[stepsRecipe];
    }

    public void shareText()
    {
        //creating the strings that will be shared
        string subject = recipe.Title;
        string body = ingredientsTitle.text + "\n";

        //filling the ingredients
        for (int i = 0; i < recipe.IngredientCant.Length; i++)
        {
            body = body + recipe.IngredientCant[i] + recipe.IngredientDesc[i] + "\n";
        }

        body = body + "\n" + stepsTitle.text + "\n";

        //filling the ingredients
        for (int i = 0; i < recipe.Steps.Length; i++)
        {
            body = body + i + ") "+recipe.Steps[i] + "\n";
        }

        body = body + "\n" + "MÁS INFORMACIÓN/MORE INFO: " +"https://play.google.com/store/apps/details?id=adanzapps.easy.recipes.ForCooking";

        //execute the below lines if being run on a Android device
        #if UNITY_ANDROID
        //Refernece of AndroidJavaClass class for intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //Refernece of AndroidJavaObject class for intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //call setAction method of the Intent object created
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //set the type of sharing that is happening
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        //add data to be passed to the other activity i.e., the data to be sent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
        //get the current activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //start the activity by sending the intent data
        currentActivity.Call("startActivity", intentObject);
        #endif
    }

    //SPANISH RECIPES
    //12 recipes
    public void ChooseDessertSP (int i)
    {
        //titulo, cantidad, palabra de cantidad
        //img, int ingredientes, string ingredientes
        //dificultad, tiempo de preparación, pasos a seguir
        switch (i)
        {
            case 1:
                //Tarta de queso y chocolate
                recipe = new Recipe("Tarta de queso y chocolate",6,"raciones","10", new int[] { 350, 150, 400, 150, 20 }, new string[] { "ml de nata para montar al 35%", "g de azúcar", "g de queso Philadelphia", "g de chocolate de cobertura al 54%", "cm sería la longitud del molde redondo que necesitaríamos" }
                                            , 3, 160, "postres"
                                            , true, false, new string[] { "Lo primero sería preparar la salsa de chocolate. Lo ponemos en el microondas en intervalos de 30 segundos y lo removemos hasta que se vuelva líquido. A continuación añadimos 150 ml de nata y dejamos reposar 1 minuto hasta que se convierta en una crema", "(opcional) si queremos hacer un bizcocho para la base deberías hacerlo ahora para que dé tiempo a enfriar", "En un bol incluímos el queso y el azúcar y removemos bien hasta que obtengas una crema, finalmente añadimos el resto de la nata y mezclamos bien", " Colocamos la mitad de la crema en el molde desmontable (encima del bizcocho si lo tenemos). La extendemos y la ponemos encima de la nevera durante 15 minutos. A continuación echamos el resto de la crema y el resto de la salsa de chocolate", "Lo ponemos en la nevera un mí­nimo de 2 horas y desmoldamos al servir" });

                break;
            case 2:
                //cupcakes de chocolate (14 unidades)							
                recipe = new Recipe("Cupcakes de chocolate", 14, "unidades","11", new int[] { 3, 1, 150, 100, 60, 135, 16, 50, 14, 300, 4, 3 }, new string[] { "huevos medianos", "yogur", "g. de azúcar", "ml de aceite de olvia", "g. de cacao sin azúcar", "g. harina de reposterí­a", "g. de levadura en polvo", "g. de chocolate negro", "g. de queso crema", "cucharadas de azúcar", "cucharadas de nata líquida", "g. de chocolate negro" }
                                            , 1, 30, "postres"
                                            , true, false, new string[] { "En un bol incluímos los huevos, el azúcar y lo batimos hasta que hagan espuma", "Mientras seguimos batiendo echamos el yogur y el aceite. También vamos incorporando el caco y removemos bien. Luego incluimos la harina en la que incluiremos una cucharadita de levadura y mezclamos bien", "Con la masa resultante echaremos 2 cucharadas en cada molde (dejando la mitad del molde sin cubrir para que salga mejor)", "A continuación podemos ponerlas en el microondas (a 3 minutos 6 unidades) o podemos hornearlas a 180 grados entre 20 y 25 minutos", "Ahora nos falta hacer la crema de chocolate: derretimos el chocolate y lo ponemos al microondas en intervalos de 30 segundos, después removemos hasta que quede líquido", "mezclamos bien el queso crema con la nata y el azúcar hasta que se vuelva con brillo, finalmente incluímos el chocolate derretido y removemos bien", "Decoramos colocando la crema en una bolsa o manga pastelera desechable colocamos la boquilla con forma de churro, cortamos en la punta y dibujamos de fuera hacia adentro. Salpicamos unas virutas de chocolate o chocolate derretido y un trozo de chocolate para decorar." });

                break;
            case 3:
                //tarta de huesitos de 6 raciones							
                recipe = new Recipe("Tarta de huesitos",6,"raciones","12", new int[] { 12, 250, 150, 50, 10, 6 }, new string[] { "huesitos", "g. de chocolate", "g. de nata o crema de leche", "g. de mermelada de fresa", "nuves", "golosinas" }
                                            , 4, 20, "postres"
                                            , true, false, new string[] { "Lo primero será fundir 150 gramos de chocolate, puede ser al baño marí­a o al microondas. Si es en el microondas lo pondremos a intervalos de 30 segundos y lo revolveremos suavemente hasta que se funda", "A continuación, ponemos 100ml de nata al fuego y mientras se calienta vamos troceandos los otros 100 g. de chocolate en trozos pequeños.","Cuando la nata rompa a hervir la retiraremos del fuego, le echaremos el chocolate y removeremos hasta que se funda. Finalmente le echaremos un par de cucharadas de mermelada de fresa y mezclamos hasta que se mezclen bien los ingredientes", "Para montar nuestra tarta la colocaremos sobre una fuente plana y haremos una base con el chocolate fundido del ancho y largo de 6 guesitos juntos uno a par del otro", "Una vez que tenemos la base colocamos los huesitos juntos. Para que se unan mojaremos uno de los lados con chocolate y presionamos con el anterior huesito", "Una vez que tenemos el primer piso de huesitos hacemos un borde por la parte exterior con el chocolate fundido y rellenamos el interior con la ganache de fresa. En este punto decidiremos si queremos darle un extra de grosor de ganache (cosa que yo recomiendo pues está buení­sima) en cuyo caso lo meterí­amos unos 5 minutos en el frigorífico", "Pasado este tiempo volvemos a echar otro borde de chocolate y a rellenar el centro con la ganache. Si no queremos este grosor extra nos saltamos este paso.", "A continuación colocamos un segundo piso de huesitos de la misma forma. Una vez finalizado el segundo piso echamos una capa de chocolate fundido por encima y la enfriaremos en el frigorífico durante 10 minutos hasta que endurezca el chocolate", "Finalmente echaremos el resto de la nata en el chocolate fundido que nos ha sobrado y, como está frí­o lo calentaremos unos 30 segundos en el microondas y removemos hasta que quede una masa fluída.", "Ahora cortamos las nubes hasta conseguir unos discos parecidos a unos labios. Quitamos la tarta del frigorífico, la soltamos del papel y cortamos los bordes que pudiesen quedar. Con ayuda de una espátula la colocamos en la fuente donde la vamos a presentar y colocamos por encima una capa de labios de nube.", "Repartimos la última ganache sobre las nubes hasta que queden cubiertas, sin preocuparnos que pueda caer algo por los bordes, eso le dará un toque de contraste muy apetecible. Servimos o reservamos en un lugar fresco. Acompañaremos con un bol donde pondremos el resto de nubes y gominolas y otro con el resto de la ganache de fresa." });
                break;
            case 4:
                //9 Tortitas caseras
                recipe = new Recipe("Tortitas caseras" ,9 ,"unidades" ,"13", new int[] { 2, 3, 1, 425, 400, 20, 50 }, new string[] { "huevos", "cucharadas de azÃºcar", "pizca de sal", "ml de leche", "harina fina de reposterÃ­a", "g. de levadura en polvo (2 sobres)", "g. de mantequilla" }
                                            , 1, 10, "postres"
                                            , true, false, new string[] { "Para hacer la masa pondremos en un bol: los huevos el azúcar y la sal. Lo batimos enérgicamente y añadiremos casi toda la leche (dejaremos 100 ml) y lo mezclamos todo de nuevo", "Lo pasamos a una jarra. Luego en un bol tamizamos (pasamos por un colador) la harina junto con la levadura en polvo, hacemos un hueco en el centro y vamos echando la mezcla liquida y removiendo con una cuchara del centro hacia afuera para que no se creen brumos."," De esta manera no se crean grumos si vemos que está demasiado líquida nunca añadiremos la harina en seco, por eso preferimos echar poco liquido y cuando la mezcla esté echa le vamos añadiendo leche hasta que esté en el punto de espesor.", "Ahora ponemos un poco de mantequilla en una sartén de unos 24 cms a fuego medio bajo y cuando esté caliente echamos un tamaño de 3 cucharadas de la masa. Cuando lleve un minuto aproximadamente le damos la vuelta (cuando coja color) dejamos otro minuto aproximadamente y vamos colocando en un plato. ", "Para decorarlo: podemos acompañarlo con nata liquida montada, sirope de fresas, chocolate o caramelo, o en su lugar un chorro de miel, unas frutas en taquitos y listo!" });
                break;
            case 5:
                // Volcán de chocolate en un minuto (para 4 personas)
                recipe = new Recipe("Volcán de chololate", 4, "personas", "14", new int[] { 125, 50, 3, 80, 1, 1 }, new string[] { "g. de cobertura de chocolate", "g. de mantequilla", "huevos", "g. de azúcar", "cucharada de harina de reposterí­a", "copita de licor de naranja" }
                                            , 2, 5, "postres"
                                            , true, false, new string[] { "Fundimos el chocolate con la mantequilla en intervalos de 30 segundos y mezclamos bien hasta que quede uniforme", "en un bol incluimos los huevos, azúcar y batimos hasta que quede espumita y quede de color amarillo claro. A continuación añadimos la mezcla de chocolate y removemos una vez más hasta conseguir una mezcla uniforme.", "Finalmente añadimos una cucharada de harina pasándola por un colador (tamizar) y le añadiremos también el licor de naranja", "Lo repartimos en diferentes cuencos dependiendo de el número de personas que lo vallan a comer. Lo ponemos al microondas 1 minuto . Lo decoramos a nuestro gusto y servimos caliente." });
                break;
            case 6:
                // Mousse de limón: para 4
                recipe = new Recipe("Mousse de limón", 4, "raciones","15", new int[] { 125, 300, 4, 1, 125, 350 }, new string[] { "ml de zumo de limón", "g. de azúcar", "yemas de huevo", "cuchara de rayadura de limón", "g. de mantequilla sin sal", "ml de nata lí­quida para montar" }
                                            , 2, 250, "postres"
                                            , true, false, new string[] { "Primero de todo tendremos que preparar el almíbar de limón: Para ello, Ponemos a calentar en un cazo 125 ml (media taza) de zumo de limón, 125 g de azúcar Hervimos a fuego lento. Mientras dejamos que se tiemple un poco montamos la nata (crema de leche), aÃ±adiendo 4 cucharadas de azúcar casi al final.", "A continuación prepararemos el mousse: Ponemos 4 yemas de huevo en un bol, añadimos 125 g. de azúcar. Vamos incorporando poco a poco el almíbar de limón y mezclandolo con las yemas, removiendo continuamente. Volvemos a echar la mezcla en el cazo y cocemos hasta que salgan burbujas.", "Lo dejamos enfriar 10 minutos y luego batimos hasta que duplique su volumen. Añadimos la ralladura de limón (1 cucharada) y 125 g de mantequilla sin sal a temperatura ambiente cortada en trozos. Añadimos los 350 g ( 1 y  1/2 taza ) de nata ya montada y mezclamos suavemente con la espátula.", "Dejamos enfriar 4 horas para que cambie su sabor y espese creandose el aspecto de mousse espumoso (como queso), listo!" });
                break;
            case 7:
                // Mousse de chocolate para 4
                recipe = new Recipe("Mousse de chocolate", 4, "personas","16", new int[] { 175, 30, 4, 80, 1 }, new string[] { "g. de cobertura de chocolate", "g. de mantequilla", "huevos", "g. de azúcar", "(opcional) una copa de ron, brandy o ralladura de naranja" }
                                            , 2, 130, "postres"
                                            , true, false, new string[] { "Primero de todo fundimos el chocolate y la mantequilla en el microondas en intervalos de 30 segundos y lo mezclaremos hasta que quede uniforme. También podemos hacerlo al baño maría", "Separamos las claras de las yemas. En el bol de las yemas añadimos el azúcar y vamos removiendo con ayuda de una espátula añadiendo la mezcla de chocolate a cucharadas poco a poco, hasta que consigamos una mezcla uniforme.", "Batimos las claras a punto de nieve y las vamos incorporando poco a poco a la anterior mezcla de chocolate. Vamos mezclando despacio con movimientos envolventes hasta conseguir que quede uniforme la mezcla. No debemos utilizar batidora. Si no lo van a tomar los niños podemos añadirle una cucharada de licor tipo coñac o ron.", "Lo repartimos en el número de cuencos que queramos y lo dejamos enfriar en la nevera para que se integre durante un par de horas" });
                break;
            case 8:
                //Bizcocho casero para 8 personas
                recipe = new Recipe("Bizcocho casero", 8, "personas","17", new int[] { 7, 1, 80, 160, 140, 1, 25, 200 }, new string[] { "huevos", "yogurt natural", "g. aceite (preferiblemente de oliva)", "g. de azúcar", "g. de harina de repostería", "sobre con levadura química", "ralladura de naranja", "uvas blancas", "g. de chocolate" }
                                            , 4, 50, "postres"
                                            , true, false, new string[] { "Precalentamos el horno a 180 grados y preparamos un molde (aconsejable 22 cm de diámetro)", "separamos en recipientes distintos las yemas de las claras y batimos las claras a punto de nieve", "A continuación batimos las yemas con el azúcar hasta formar una crema, añadimos el yogurt y batimos, el aceite y batimos, a partir de este momento apagamos la batidora eléctrica.", "A continuación mezclamos la harina con la levadura y la pasamos por un colador incorporando a la mezcla y removiendo con una espátula o cuchara. Echamos la mitad de la clara a punto de nieve y mezclamos suavemente.", "Añadimos el resto y terminamos de mezclar con suaves movimientos envolventes para que no baje la mezcla. Vertemos en el molde y dejamos caer las uvas peladas sin pepita repartidas por la mezcla. Horneamos a 180 grados durante 30 o 35 minutos.", "Cuando saquemos del horno el bizcocho lo dejamos enfriar durante unos 30 minutos que reposará en el molde. Una vez frí­a quedará prácticamente suelta de los bordes y sólo habrí­a que desmoldarla, quitarle el papel de hornear y presentarlo en la fuente.", "Entonces fundimos los 200 g de chocolate de cobertura al baño marí­a o en el microondas 30 seg, removemos bien y echamos por encima del bizcocho, llegados a este momento pasamos por encima un tenedor haciendo un dibujo de ondas (como el tronco de navidad) y dejamos enfriar. Colocamos las virutas encima y enfriamos" });
                break;
            case 9:
                //Galletas de almendra (para 4 personas)
                recipe = new Recipe("Galletas de almendra",4,"personas","18", new int[] { 120, 100, 50, 1, 300, 16 }, new string[] { "g. de mantequilla", "g. de azúcar moreno (puede ser azúcar blanco si no tenemos)", "huevo", "g. de harina de repostería", "g. de levadura", "almendras en láminas" }
                                            , 3, 90, "postres"
                                            , true, true, new string[] { "En un bol metemos el huevo y lo batimos bien, a continuación añadiremos la mantequilla (la calentamos un poco si es necesario para que no quede espesa) y los dos tipos de azúcar para mezclarlo todo", "Ahora sobre una fuente colocamos un papel de hornear ponemos la masa en el centro y colocamos encima otro papel de hornear. Con ayuda de un rodillo extendemos la masa dejando un grosor aproximado de 3 monedas. Dejamos enfriar en el frigorífico durante una hora.", "Mientras tanto precalentamos el horno a 160ºC, quitamos el papel de la parte superior y lo colocamos como base en la bandeja en la que vamos a hornear. Vamos cortando la masa con un cortapastas o un vaso y colocamos las galletas en la fuente en la que vamos a hornear.", "Juntamos los recortes que nos quedan de masa y la volvemos a extender para hacer mas galletas hasta que se acabe.", "Finalmente repartimos las láminas de almendra por encima de las galletas presionando ligeramente. Horneamos a 160ºC durante 15-20 minutos hasta que estén doradas. retiramos dejamos enfriar y servimos" });
                break;
            case 10:
                //Gominolas caseras
                recipe = new Recipe("Gominolas caseras",6,"Personas","19", new int[] { 200, 300, 2, 1, 1, 1 }, new string[] { "ml de agua (se puede cambiar por zumo o leche)", "g. de azúcar", "sobres de gelatina neutra", "sobres de gelatina sabor fresa", "poco de azúcar para el rebozado", "poco de aceite" }
                                            , 1, 250, "postres"
                                            , false, false, new string[] { "Primero pintamos los moldes de silicona o el recipiente donde pondremos las gominolas con aceite", "A continuación calentamos en un cazo el agua (zumo o leche) y añadiremos también los dos sobres de gelatina neutra y removemos. Cuando este diluída incluiremos también la gelatina de sabor y el azúcar. Finalmente lo dejaremos al fuego unos 4 minutos sin dejar de remover", "Sin dejarlo enfriar vertemos el líquido en los moldes de silicona.  Debemos esperar unas 6 horas hasta que las gominas se hayan enfriado. Si tenemos un poco de prisa se pueden dejar una hora menos  en la nevera.", "Finalmente desmontamos en gominolas y rebozamos en el azÃºcar" });
                break;
            case 11:
                //Leche frita
                recipe = new Recipe("Leche frita",6,"raciones","20", new int[] { 1, 1, 3, 1, 80, 50, 1 }, new string[] { "medio litro de leche", "medio palo de vainilla o monda de limón", "yemas de huevo", "huevo", "g. de azúcar", "g. de harina", "pizca de mantequilla para untar por encima" }
                                            , 2, 10, "postres"
                                            , true, false, new string[] { "Primero herviremos la leche con los aromas que le van a dar sabor, en este caso serí­a la vainilla pero podemos usar por ejemplo una monda de limón. Cuando hierva lo dejaremos templar", "En un bol aparte ponemos el azúcar con la harina removemos y echamos un poco de leche templada y añadimos las yemas y el huevo batidos y removemos bien.", "Agrega a esta mezcla al resto de la leche y calentamos a fuego medio hasta que espese sin parar de remover.", "Enfríalo en un recipiente y coloca la mantequilla como una capa por encima." });
                break;
            case 12:
                //Flan de huevo casero
                recipe = new Recipe("Flan", 3, "raciones","21", new int[] { 4, 4, 400, 1 }, new string[] { "huevos", "cucharadas colmadas de azúcar", "ml de nata líquida (o crema de leche)", "poco de canela y limon (también podéis usar naranja coñac o whisky)" }
                                            , 2, 35, "postres"
                                            , true, false, new string[] { "Incluiremos los ingredientes por el siguiente orden: huevos, azúcar, leche, canela y limón y vamos removiendo.", "En otra tartera (que quepa en otra olla más grande) se hace caramelo con 10 cucharadas de azúcar y 1/2 chupito de agua para que moje el azúcar (no remover con la cuchara ). Se deja hacer hasta que adquiera su color tostado y espese.", "Una vez está el caramelo se echa lo batido, y este recipiente se pone dentro de la tartera  grande en la que previamente se echó agua suficiente para que quede por encima del nivel del flan. Se deja estar entre 25 y 30  minutos." });
                break;

        }

        return;

    }
    //11 recipes
    public void ChooseAppetizerSP (int i)
    {
        //español
        switch (i)
        {
            case 1:
                //Almejas al ajillo
                recipe = new Recipe("Almejas al ajillo", 2, "tapas", "40", new int[] { 20, 2, 1, 1, 1, 3 }, new string[] { "Almejas", "Dientes de ajo", "Perejil picado para decorar", "Aceite de oliva virgen extra", "Guindilla al gusto", "Sal" }
                        , 1, 15, "aperitivo"
                        , true, false, new string[] { "Dejamos las almejas en agua con sal durante unas horas, para que eliminen la arena que puedan tener dentro.", "A continuación, laminamos los dientes de ajo y cortamos un trozo de guindilla y los ponemos a infusionar en el aceite templado, con el fuego al mínimo para evitar que se doren los ajos muy rápido.", "Agregamos las almejas y tapamos la sartén para que el vapor ayude a que las almejas se vayan abriendo.", "Mientras se abren todas, vamos picando perejil, que utilizaremos en el último momento para espolvorear sobre las almejas al ajillo, que ya habrán terminado de abrirse y ¡estarán listas para la degustación!" });
                break;
            case 2:
                //Gofres caseros: cantidad: entre 6 y 8
                recipe = new Recipe("Gofres caseros", 7, "unidades", "0", new int[] { 150, 8, 30, 1, 200, 40 }, new string[] { "g. de harina de repostería", "g. de levadura en polvo", "g. de azúcar", "huevo", "ml de nata", "g. de mantequilla" }
                        , 3, 35, "aperitivo"
                        , true, false, new string[] { "Primero separamos la clara de la yema en 2 recipientes diferentes y montamos la clara al punto de nieve.", "A la yema le añadimos levadura mezclada con harina, nata líquida o leche si no tenemos, azúcar y mantequilla. Necesitaremos mezclarlo todo enérgicamente y lo dejamos reposar entre 20 y 30 minutos", "Calentamos la gofrera y echamos la cantidad necesaria de nuestra mezcla. Cerramos la gofrera y le damos el tiempo de cocción que necesite, en nuestro caso 4-5 minutos", "Retiramos los gofres de la gofrera y listo! ahora podemos acompañarlos de chocolate, nata, mermelada, sirope o lo que queramos!" });
                break;

            case 3:
                //pastél japonés para 4 personas
                recipe = new Recipe("pastél Japonés", 4, "personas", "1", new int[] { 3, 120, 120, 1 }, new string[] { "huevos", "g. de chocolate blanco", "g. de queso en crema", "poco de azúcar glaseado para decorar" }
                        , 3, 45, "aperitivo"
                        , true, false, new string[] { "Primero de todo ponemos a precalentar el horno a 170 grados y vamos separando las yemas de las claras", "Echamos el chocolate blanco en un cuenco y lo derretimos al microondas en intervamos de 15 segundos y lo removemos hasta que se deshaga. A continuación añadiremos el queso en crema, revolvemos bien y finalmente añadiremos también las yemas para luego volver a remover", "Ahora batimos las claras hasta que queden al punto de nieve y las añadiremos a la mezcla", "Echamos la mezcla en un molde de unos 20 cms y lo ponemos sobre la bandeja del horno en la que hemos echado agua (baño maria).", "Dejamos en el horno el pastel a 170 grados durante 15 minutos. Pasado ese tiempo bajamos la temperatura del horno a 160 grados y lo dejamos otros 15 minutos. Por último, apagamos el horno pero no sacamos el pastel porque lo dejamos otros 15 minutos más.", "Cuando lo emplatemos lo espolvoreamos con azúcar glass y dejamos que se enfríe." });
                break;

            case 4:
                //canapés tropicales 4 raciones
                recipe = new Recipe("Canapés Tropicales", 4, "raciones", "2", new int[] { 1, 1, 1, 1, 1, 1, 8, 1, 1, 4 }, new string[] { "aguacate", "mango", "bote de habas rojas", "pimiento", "guindilla", "cebolleta", "tortillas de trigo o maíz", "lima", "poco de aceite de oliva o girasol", "vasos o moldes para ornear las fajitas" }
                        , 2, 5, "aperitivo"
                        , true, false, new string[] { "Primero cortamos las fajitas en 4 partes y las colocamos en los moldes. Presionaremos la parte del medio de manera que se formen una especie de cestos. Con un pincel o brocha de cocina, mojamos un poco el interior de las Tortillas. Precalentamos el horno a 180ºC y horneamos las fajitas en los moldes durante 8 minutos.", "Cortamos el pimiento, la cebolleta y la Guindilla en cuadrados pequeños. Partimos el aguacate a la mitad y le quitamos el corazón; cortamos en cuadrados sin llegar a la piel y le quitamos la piel. Hacemos lo mismo con el Mango.", " En un bol grande ponemos el bote de Habas Rojas escurrido, el pimiento, la cebolleta, la guindilla, el aguacate y el mango. Exprimimos una lima por encima de los ingredientes y mezclamos todo bien. Rellenamos nuestras cestas con los ingredientes y listo!" });
                break;

            case 5:
                //patatas asadas al microondas en solo 3 minutos! (2 personas)
                recipe = new Recipe("patatas asadas \n al microondas", 2, "personas", "3", new int[] { 4, 1, 1, 1 }, new string[] { "patatas pequeñas", "chorro de aceite de oliva", "pizca de pimienta negra", "papel de plata" }
                        , 1, 5, "aperitivo"
                        , true, false, new string[] { "Primero extenderemos el papel de hornear en el centro y ponemos un chorrito de aceite. Colocaremos encima unas patatas pequeñas y salpicamos un poco con pimienta negra. Finalmente le volvemos a echar otro chorrito de aceite", "Envolvemos para que no salga el vapor. Ponemos la abertura del papel cara abajo en el plato o fuente donde lo vayamos a cocinar.", "Lo colocamos en el microondas a temperatura media (aprox 650 watios) durante 3 minutos, sacamos, damos la vuelta y ponemos medio minuto mas." });
                break;

            case 6:
                //Ensalada de tomate
                recipe = new Recipe("Ensalada de tomate", 2, "raciones pequeñas", "4", new int[] { 3, 1, 1, 1, 1 }, new string[] { "cucharas de aceite de oliva", "1/4 taza de vinagre balsámico", "caja de tomates lavados", "1/4 taza de hojas de albahaca", "un poco de sal gruesa", "poco de pimienta recién molida" }
                        , 1, 3, "aperitivos"
                        , true, false, new string[] { "Primero calentaremos el vinagre y el aceite de oliva hasta que empiece a hacer burbujas (suele tardar unos 2 minutos aproximadamente)", "A continuación mezclaremos con el líquido recién calentado: la sal, la pimienta y la albahaca y la dejamos reposar antes de servir", "Justo antes de servir deberemos cojer el resto del vinagre y rociaremos la ensalada con ello, finalmente le añadiremos una pizca de sal gruesa" });
                break;

            case 7:
                //Ensalada caprese
                recipe = new Recipe("Ensalada carpese", 2, "raciones pequeñas", "5", new int[] { 1, 3, 1, 1, 1 }, new string[] { "poco de mozzarella fresca", "tomates maduros", "manojo de albahaca", "poco de aceite de oliva virgen", "poco de sal marina pimienta negro" }
                        , 1, 2, "aperitivos"
                        , true, false, new string[] { "Primero de todo cortaremos los tomates en finas lonchas colocándolas encima de una bandeja una a una", "A continuación pondremos sobre cada rodaja de tomate una hoja de albahaca y una capa de mozzarella", "Finalmente rocíamos la ensalada con aceite de oliva y la enriquecemos con un poquito de sal y pimienta negra para que cuando nos la comamos esté 100% fresca" });
                break;

            case 8:
                //Tomate y ensalada de maíz
                recipe = new Recipe("Ensalada de tomate y maíz", 2, "raciones pequeñas", "4", new int[] { 1, 4, 1, 1, 1, 1 }, new string[] { "poco de maíz fresco y dulce", "tomates medianos y maduros", "1/4 de taza de hojas de menta fresca", "1/4 de taza de hierbas frescas (puede ser de perejil, albahaca, romero, salvia y una cucharada de aceite de oliva)", "poco sal y pimienta fresca negra", "poco de queso de cabra fresco" }
                        , 2, 6, "aperitivos"
                        , true, false, new string[] { "Primero de todo deberás poner agua en una olla grande, y cuando llegue a la ebullición añade el maíz y lo dejaremos hervir durante 5 minutos", "Mientras el maíz está hirviendo cortaremos los tomates en cuartos o en lonchas. Rasparemos o exprimieremos además el zumo y eliminaremos la humedad restante con un poco de papel", "Una vez el maíz se halla enfriado pondremos cada mazorca de pie encima de un recipiente ancho y poco profundo. Finalmente mezclaremos los granos de maíz con un cuchillo afilado y poco profundo para mezclar el maíz y los tomates", "Finalmente añadiremos las hojas de menta , las hierbas, el aceite de oliva, la sal y la pimienta. Y listo!" });
                break;
            case 9:
                //Ensalada de patata
                recipe = new Recipe("Ensalada de patata", 6, "raciones", "6", new int[] { 1, 2, 6, 15, 100, 1, 1, 1, 1, 1, 1 }, new string[] { "Kg. de patatitas baby mediterráneas, cocidas y peladas", "cucharadas pequeñas de comino tostado semillas", "cebollas rojas para ensalada", "g. de menta fresca", "g. de granada", "1/4 taza de aceite", "1/2 taza de mayonesa", "pizca de azafrán en un poco de agua caliente", " medio limón", "poco de sal marina", "poco de pimienta negro" }
                        , 2, 10, "aperitivos"
                        , true, false, new string[] { "Cortamos las patatas por la mitad y las colocamos en un recipiente tras añadir las cebollas rojas y las semillas de comino", "Mezclaremos los ingredientes del aderezo y los vertiremos sobre las patatas para que coja sabor", "Seguimos adornándolo con hojas de menta fresca y semillas de granada" });
                break;
            case 10:
                //pincho de salmón
                //dificultad 1 y tiempo de preparación 3 minutos
                recipe = new Recipe("Pincho de salmón", 4, "raciones", "7", new int[] { 4, 4, 1, 1, 1 }, new string[] { "tostadas de pan", "hojas de lechuga", "poco de salmón ahumado", "poco de gambas cocidas y peladas", "poco de mayonesa" }
                        , 1, 2, "aperitivos"
                        , true, false, new string[] { "Primero cortaremos la lechuga en pequeños trozos y la metemos en un bol, además añadiremos el salmón y las gambas en el mismo", "Finalmente lo mezclamos bien y echamos la mezcla sobre las tostas de pan" });
                break;
            case 11:
                //pincho de gambas empanadas
                //dificultad 2 tiempo de preparación 25 minutos
                recipe = new Recipe("Pincho de gambas \n empanadas", 12, "raciones pequeñas", "8", new int[] { 12, 1, 1, 1, 1, 1, 1 }, new string[] { "gambas", "poco de sal", "poco de pimienta blanca", "harina de trigo", "huevo batido", "poco de pan rallado", "poco de aceite para freir" }
                        , 2, 5, "aperitivos"
                        , false, true, new string[] { "Lo primero será pelar las gambas quitándoles la cabeza y la piel. A continuación le añadiremos sal y pimienta", "Las pincharemos en pinchos de madera y las reboramos. Primero con harina y luego con huevo batido y finalmente con pan rallado", "Las freímos en aceite y listo!" });
                break;
            case 12:
                //pinchos de jamón tomate y queso
                //dificultad 1, tiempo 3 minutos
                recipe = new Recipe("Pincho jamón, queso \n y tomate cherry", 8, "unidades", "9", new int[] { 30, 30, 30, 1, 1 }, new string[] { "cuadraditos de queso (aproximadamente)", "cuadraditos de jamón cocido (aproximadamente)", "tomates cherry", "lechuga iceberg", "palitos de madera o de colores" }
                        , 1, 3, "aperitivos"
                        , true, false, new string[] { "Primero de todo tendremos que lavar los tomates y las hojas de lechuga, para después dejarlas secar", "En cada uno de los palitos insertaremos: 1 cuadradito de queso, 1 cuadradito de jamón y un tomate cherry", "Para la decoración pondremos en el medio del plato la hoja de lechuga y unos cuantos tomates cherry encima de la lechuga", "Colocaremos los pinchos alrededor de la lechuga. Y listo!" });
                break;
        }

    }
    //13 recipes
    public void ChooseDietSP (int i)
    {
        //español
        switch (i)
        {
            case 1:
                //Ensalada de tomate
                recipe = new Recipe("Ensalada de tomate", 2, "raciones pequeñas", "4", new int[] { 3, 1, 1, 1, 1 }, new string[] { "cucharas de aceite de oliva", "1/4 taza de vinagre balsámico", "caja de tomates lavados", "1/4 taza de hojas de albahaca", "un poco de sal gruesa", "poco de pimienta recién molida" }
                        , 1, 3, "dieta"
                        , true, false, new string[] { "Primero calentaremos el vinagre y el aceite de oliva hasta que empiece a hacer burbujas (suele tardar unos 2 minutos aproximadamente)", "A continuación mezclaremos con el líquido recién calentado: la sal, la pimienta y la albahaca y la dejamos reposar antes de servir", "Justo antes de servir deberemos cojer el resto del vinagre y rociaremos la ensalada con ello, finalmente le añadiremos una pizca de sal gruesa" });
                break;

            case 2:
                //Ensalada caprese
                recipe = new Recipe("Ensalada carpese", 2, "raciones pequeñas", "5", new int[] { 1, 3, 1, 1, 1 }, new string[] { "poco de mozzarella fresca", "tomates maduros", "manojo de albahaca", "poco de aceite de oliva virgen", "poco de sal marina pimienta negro" }
                        , 1, 2, "dieta"
                        , true, false, new string[] { "Primero de todo cortaremos los tomates en finas lonchas colocándolas encima de una bandeja una a una", "A continuación pondremos sobre cada rodaja de tomate una hoja de albahaca y una capa de mozzarella", "Finalmente rocíamos la ensalada con aceite de oliva y la enriquecemos con un poquito de sal y pimienta negra para que cuando nos la comamos esté 100% fresca" });
                break;

            case 3:
                //Tomate y ensalada de maíz
                recipe = new Recipe("Ensalada de tomate y maíz", 2, "raciones pequeñas", "4", new int[] { 1, 4, 1, 1, 1, 1 }, new string[] { "poco de maíz fresco y dulce", "tomates medianos y maduros", "1/4 de taza de hojas de menta fresca", "1/4 de taza de hierbas frescas (puede ser de perejil, albahaca, romero, salvia y una cucharada de aceite de oliva)", "poco sal y pimienta fresca negra", "poco de queso de cabra fresco" }
                        , 2, 6, "dieta"
                        , true, false, new string[] { "Primero de todo deberás poner agua en una olla grande, y cuando llegue a la ebullición añade el maíz y lo dejaremos hervir durante 5 minutos", "Mientras el maíz está hirviendo cortaremos los tomates en cuartos o en lonchas. Rasparemos o exprimieremos además el zumo y eliminaremos la humedad restante con un poco de papel", "Una vez el maíz se halla enfriado pondremos cada mazorca de pie encima de un recipiente ancho y poco profundo. Finalmente mezclaremos los granos de maíz con un cuchillo afilado y poco profundo para mezclar el maíz y los tomates", "Finalmente añadiremos las hojas de menta , las hierbas, el aceite de oliva, la sal y la pimienta. Y listo!" });
                break;
            case 4:
                //Ensalada de patata
                recipe = new Recipe("Ensalada de patata", 6, "raciones", "6", new int[] { 1, 2, 6, 15, 100, 1, 1, 1, 1, 1, 1 }, new string[] { "Kg. de patatitas baby mediterráneas, cocidas y peladas", "cucharadas pequeñas de comino tostado semillas", "cebollas rojas para ensalada", "g. de menta fresca", "g. de granada", "1/4 taza de aceite", "1/2 taza de mayonesa", "pizca de azafrán en un poco de agua caliente", " medio limón", "poco de sal marina", "poco de pimienta negro" }
                        , 2, 10, "dieta"
                        , true, false, new string[] { "cortamos las patatas por la mitad y las colocamos en un recipiente tras añadir las cebollas rojas y las semillas de comino", "mezclaremos los ingredientes del aderezo y los vertiremos sobre las papas para que coja sabor", "Seguimos adornándolo con hojas de menta fresca y semillas de granada" });
                break;
            case 5:
                //Pasta con salmón y pesto
                //dificultad fácil-media, tiempo 40 minutos y 660 calorías por porción
                recipe = new Recipe("Pasta con salmón y pesto", 2, "raciones", "22", new int[] { 1, 25, 50, 1, 1, 3, 1, 1, 175, 50, 100, 100 }, new string[] { "1/2 diente de ajo", "g. de queso parmesano", "ml. de zumo de limón", "pequeño manojo de perejil", "manojo de albahaca", "cucharadas de aceite de oliva", "poco de sal", "poco de pimienta", "g. de pasta", "g. de rúcula", "g. de tomates cherry", "g. de salmón ahumado" }
                        , 2, 10, "dieta"
                        , true, false, new string[] { "Primero hay que pelar los ajos y cortarlos en trozos grandes", "A continuación rallamos el parmesano en una rejilla gruesa", "Ahora haremos un puré con: el queso parmesano, las hierbas, el ajo y el aceite. Añadiremos también el zumo de limón, la sal y la pimienta", "ponemos a cocer la pasta entre 5 y 10 minutos", "mientras tanto lavaremos la rúcula, partimos los tomates a la mitad, cortamos el salmón en tiras, calentamos algo de aceite en una sartén y freímos los tomates en el mismo junto con los filetitos de salmón durante 1 minuto", "Finalmente añadimos wl pesto y la rúcula a la pasta terminada y mezclamos bien todos los ingredientes que nos faltan" });
                break;
            case 6:
                //Palitos de pescado con crema de yogurt
                //dificultad fácil, 40 minutos, 550 kcal por porción
                recipe = new Recipe("Palitos de pescado \n con crema de yogurt", 2, "raciones", "23", new int[] { 500, 2, 2, 1, 225, 1, 1, 1, 1, 1, 1 }, new string[] { "g. de patatas harinosas", "cucharadas de salsa para la ensalada", "cucharadas de yogurt natural", "algunos cebollinos", "g. de barritas de pescado (8 piezas)", "poco de leche", "cuchara de margarina", "poco de nuez moscada rallada", "lata de maíz con frijoles", "poco de sal", "poco de pimienta" }
                        , 2, 30, "dieta"
                        , true, false, new string[] { "Primero pelaremos las patatas y las hervimos con sal durante 20 minutos", "Mientras tanto mezclaremos la salsa para ensalada con el yogurt y le añadimos además sal y pimienta. Además también lavamos y cortamos las cebolletas y las juntaremos con la crema", "Mientras las patatas se siguen cociendo, descongelaremos los palitos de pescado y los freiremos en aceite", "A continuación escurrimos las patatas una vez cocidas, mezclamos además la leche y las cucharadas de margarina y se las añadimos a las patatas haciendo un puré", "Sazonamos al gusto con sal y nuez moscada", "Finalmente lavamos y escurrimos el resto de las hortalizas para ponerlas junto al puré" });
                break;
            case 7:
                //ensalada de arroz y pollo
                //tiempo 60 minutos dificultad fácul y 600 kcal por ración
                recipe = new Recipe("Ensalada de arroz", 2, "raciones", "24", new int[] { 1, 1, 1, 1, 250, 200, 1, 100, 1 }, new string[] { "cucharada de salsa de soja", "cucharada de zumo de naranja", "pieza de jengibre", "diente de ajo", "g. de filete de pechuga de pollo (aproximadamente)", "g. de arroz", "zanahoria", "g. de brotes de frijoles", "poco de aceite de oliva, sal y pimienta" }
                        , 2, 60, "dieta"
                        , true, false, new string[] { "Primero mezclamos la salsa de soja con un poco de sal y el zumo de naranja", "A continuación pelamos y cortamos el ajo y el jengibre. Para luego ponerlo en la sartén con un poco de aceite hasta que se hagan un poco", "Ahora lavaremos la carne, la secamos y la cortaremos en tiras finas", "Ahora ponemos el arroz a cocer y cuando el agua empiece a evaporarse apagamos el horno", "pelamos y cortamos las zanahorias, enjuagamos los brotes y además freiremos un minuto la carne en la sartén con aceite y la retiramos", "freiremos también las verduras en ese aceite durante un minuto", "Añadir el resto de los ingredientes (menos el arroz) junto con unos 80 ml de agua y dejar cocer a fuego lento hasta llevar a ebullición", "Finalmente salpimentamos y servimos el arroz" });
                break;
            case 8:
                //Verduras frescas al horno
                //dificultad fácil, 75 minutos 290 kcal por porción
                recipe = new Recipe("Verduras frescas al horno", 2, "raciones", "25", new int[] { 350, 1, 1, 1, 2, 1, 3, 3 }, new string[] { "g. patatas", "poco de aceite de oliva", "diente de ajo", "poco de sal y pimienta", "zanahorias", "calabacín", "tallos de mejorana", "tomates" }
                        , 2, 75, "dieta"
                        , true, false, new string[] { "Lavamos las patatas, las pelamos y las cortamos", "Untamos con aceite una bandeja, para después hornear las patatas por encima, picamos el ajo, lo añadimos y echaremos un poco de sal y aceite por encima", "ponemos el horno a precalentar a 200 grados, y luego introduciremos las patatas durante 50 minutos", "Mientras tanto: Limpiamos y pelamos las verduras y cortamos en trozos el calabacín y las zanahorias. Finalmente unos 15 minutos después de introducir las patatas añadimos todas las verduras", "Finalmente unos 2 minutos antes de que terminen de hornearse las patatas y verduras introduciremos también los tomates previamente cortados y la mejorana picada", "Añadiremos sal y pimenta y listo!" });
                break;

            case 9:
                //Batido tropical
                //muy fácil, 8 minutos
                recipe = new Recipe("Batido tropical", 2, "vasos grandes", "26", new int[] { 2, 4, 4, 4, 8 }, new string[] { "tazas de zumo", "trozos de mango", "trozos de piña", "trozos de melón", "cubitos de hielo" }
                        , 1, 8, "dieta"
                        , true, false, new string[] { "Primero cortamos los trozos de mango, piña y melón.", "También exprimiermos las naranjas hasta conseguir la cantidad necesárea y picaremos también los cubitos de hielo", "Ponemos todo el la licuadora y lo liquamos hasta que no quede ningún grumo (si no tenemos licuadora también podemos usar una batidora). Finalmente le podemos añadir algo de azúcar" });
                break;
            case 10:
                //batido rojo con avena
                //muy fácil, 2 minutos
                recipe = new Recipe("Batido rojo avena", 2, "vasos grandes", "27", new int[] { 2, 4, 1 }, new string[] { "Tazas de leche de avena", "cucharadas de mezcla de frutos rojos (pueden ser frambuesa, mora, fresa, cereza ...)", "azúcar moreno" }
                        , 1, 2, "dieta"
                        , true, false, new string[] { "Ponemos todos los ingredientes en la licuadora y los licuamos hasta que quede fina la mezcla y listo! (si no tenemos licuadora también podemos usar una batidora)" });
                break;
            case 11:
                //batido de plátano y fresa
                //muy fácil, 2 minutos
                recipe = new Recipe("Batido de fresa y plátano", 2, "vasos grandes", "28", new int[] { 8, 2, 2, 1 }, new string[] { "fresas", "plátanos", "tazas de leche de avellanas (puedes usar leche normal en su defecto)", "pizca de canela" }
                        , 1, 2, "dieta"
                        , true, false, new string[] { "[Opcional] Previamente podemos poner la fruta en la nevera o congelador para que nos quede un batido más fresco y más rico", "Ponemos todos los ingredientes en la licuadora y los licuamos hasta que quede una masa uniforme y sin grumos (si no tenemos licuadora también podemos usar una batidora)" });
                break;
            case 12:
            //batido de papaya
            //muy fácil, 5 minutos
                recipe = new Recipe("Batido de papaya", 2, "vasos grandes", "29", new int[] { 1, 2, 2, 2 }, new string[] { "Papaya", "tazas de leche de avena", "pizcas de canela", "hielos" }
                    , 1, 5, "dieta"
                    , true, false, new string[] { "Pelamos la papaya y la ponemos en la batidora o licuadora junto con la leche de avena y la canela. La trituramos bien hasta que no queden grumos" });
                break;
            case 13:
                //batido de té rojo
                //muy fácil 2 minutos
                recipe = new Recipe("Batido de té rojo", 2, "vasos grandes", "27", new int[] { 2, 1, 1, 1 }, new string[] { "tazas de té rojo", "taza de fresas o frambuesa", "cucharada de linaza o chia", "para endulzar podemos poner un poco de miel, azúcar o azúcar moreno" }
                        , 1, 2, "dietas"
                        , true, false, new string[] { "Licuamos todo o lo mezclamos con una batidora si no tenemos licuadora hasta que nos quede una mezcla uniforme", "Vertemos la mezcla resultante en los vasos y los dejamos enfriar en la nevera" });
                break;
        }

    }
    //11 recipes
    public void ChooseThermomixSP (int i)
    {
        //español
        switch (i)
        {
            case 1:
                //Arroz con leche thermomix
                recipe = new Recipe("Arroz con leche", 6, "raciones", "30", new int[] { 1, 2, 1, 1, 1, 1, 1, 1 }, new string[] { "Litro de leche", "tazas pequeñas de arroz", "taza pequeña de azúcar", "poco de sal", "rama de canela", "poco de canela molida", "corteza de limón", "Anís" }
                    , 3, 45, "Thermomix"
                    , true, false, new string[] { "Incluimos la leche, el palito de canela, la sal y la corteza de limón y la ponemos a 8 minutos con velocidad 1 y temperatura 100 grados", "Luego añadimos el arroz y ponemos 30 minutos a velocidad 1 y 100 grados igualmente", "Finalmente añadimos el azúcar y el anis y lo ponemos a mas de 100 grados con velocidad 1 durante 5 minutos. Y luego debería de estar listo para tomar" });
                break;
            case 2:
                //Flan de huevo casero
                recipe = new Recipe("Puré de calabaza", 2, "raciones", "31", new int[] { 1, 1, 2, 1, 250, 1, 2, 1, 1 }, new string[] { "Calabacín", "cebolla", "zanahorias", "1/2 calabaza (de las alargaditas)", "ml. de agua", "sal", "(opcional) quesitos en porciones", "Poco de pan del día anterior", "aceite de oliva" }
                    , 4, 40, "Thermomix"
                    , true, false, new string[] { "Primero cortamos el pan en finos taquitos y lo freiremos en la sartén sin demasiado aceite, a continuación lo dejamos hasta que se queden doraditos y removemos", "Luego picamos la cebolla en el thermomix 4 segundos con velocidad 5 y añadimos aceite para luego cocerlo a 100 grados con velocidad 1 durante 5 minutos", "Añadimos la verdura y programamos 2 minutos a velocidad 1 y 100 grados", "Entonces añadimos el agua y lo dejamos durante 25 minutos, varoma, velocidad cuchara", "opcionalmente podemos añadir un par de quesitos y los trituramos a velocidad 6 para conseguir la textura deseada" });
                break;
            case 3:
                //Hamburgesas caseras para niños
                //tiempo de cocinado 40m , dificultad 1/5
                //350 calorías por unidad
                recipe = new Recipe("Hamburguesas caseras \n para niños", 6, "raciones", "32", new int[] { 470, 50, 80, 35, 1, 1, 100, 10, 10, 0 }, new string[] { "g. de filetes de cerdo", "g. de pan", "g. de leche", "g. de cebolla", "salchica (85g. aproximadamente)", "huevo", "g. de patata cocida", "g. de salsa de soja", "g. mostaza", "un poco de aceite para freír" }
                    , 3, 40, "Thermomix"
                    , true, false, new string[] { "Primero ponemos el pan troceado en un bol y lo cubriremos con leche", "A continuación echamos en un vaso la carne en trozos, con la salchica y la cebolla. Y los picamos con varios golpes de turbo", "Acto seguido, añadiremos el pan mojado en leche, el huevo, la patata cocida, la mostaza y la soja. Y lo programaremos a velocidad 6 durante 10 segundos", "Finalmente le damos forma a las hamburguesas y las freimos con poco aceite en una sartén hasta que queden tostaditas" });
                break;
            case 4:
                //dulce de leche
                //tiempo de cocinado 50 minutos, dificultad 1/5
                recipe = new Recipe("Dulce de leche \n casero con thermomix", 4, "raciones", "33", new int[] { 380, 320, 40, 1 }, new string[] { "g. de leche condensada", "g. de leche evaporada", "g. de caramelo líquido", "1/4 cucharada de tamaño postre de bicarbonato" }
                    , 3, 50, "Thermomix"
                    , true, false, new string[] { "Para empezar ponemos todos los ingredientes dentro del thermomix y programamos 40 minutos en velocidad 3 y medio con temperatura varoma. Para facilitar la evaporación quitaremos el cubilete", "A continuación programamos 10 minutos con velocidad 3 y medio, pero esta vez sin temperatura. Para que valla enfriando", "Finalmente lo vertemos todo en un recipiente amplio. Par poder removerlo de vez en cuando mientras se enfría. Cuando consigamos que quede a temperatura ambiente lo guardaremos en la nevera hasta el momento de servir" });
                break;
            case 5:
                //Potitos de pera, 3 horas y 14 minutos, dificultad 2 / 5 y 100 kilocalorías
                recipe = new Recipe("Potitos facilísimos de pera", 2, "raciones", "31", new int[] { 400, 1 }, new string[] { "g. de pera ya pelada y sin pepitas", "zumo de 1/4 de naranja" }
                    , 5, 195, "Thermomix"
                    , false, false, new string[] { "Primero pelaremos y le quitamos las pepitas a las peras y nos aseguraremos de que están limpias antes de introducirlas en el thermomix. A continuación lo programamos a 10 minutos, temperatura varoma y velocidad 1", "Luego añadiremos el zumo de naranja y lo programamos a 4 minutos, temperatura varoma y velocidad 1", "Finalmente lo trituraremos durante 1 minuto con velocidad progresiva desde 7 a 9", "Lo dejaremos enfriar y listo!" });
                break;
            case 6:
                //patatas con merluza
                //4 raciones, 40 minutos dificultad 2/5, 300 kcal por ración
                recipe = new Recipe("Patatas con merluza", 4, "raciones", "34", new int[] { 600, 2, 350, 0, 300, 10, 30, 0 }, new string[] { "g. de agua", "puerros (solamente utilizaremos la parte blanca)", "patatas, tendremos que pelarlas y cortarlas en trozos grandes", "Un poco de sal", "g. de merluza sin piel ni espinas en trozos medianos", "colas de gambas o langostinos pelados", "g. de aceite de oliva", "perejil" }
                    , 4, 40, "Thermomix"
                    , true, false, new string[] { "Primero ponemos en el thermo los puerros con agua y lo programamos 15 minutos a 100 grados, giro a la izquierda con velocidad cuchara", "Añadimos la sal y las patatas y lo programaremos a 15 minutos, 100 grados y girlo ala izquierda con velocidad cuchara", "Finalmente incorporamos el pescado, el perejil y los langostinos o gambas y el aceite. Lo programaremos a 2 minutos, temperatura varoma, giro a la izquierda y velocidad cuchara para luego dejarlo reposar durante 5 minutos antes de servir" });
                break;
            case 7:
                //Sopa de pescado, dificultad 2/5, 50 minutos de preparación
                recipe = new Recipe("Sopa de pescado", 6, "raciones", "35", new int[] { 1000, 200, 100, 500, 200, 100, 1 }, new string[] { "g. de agua", "g. de rape", "g. de gambas", "g. de mejillones limpios y lavados con agua templada", "g. de almejas o chirlas remojadas con agua templada y sal", "g. de zanahorias", "puerro troceado" }
                    , 4, 50, "Thermomix"
                    , true, false, new string[] { "Primero introduciremos en el thermomix. Los huesos del rape, las cascaras y las cabezas de gamba (nos guardamos la cola) Sitúe el recipiente Varoma en su posición con los mejillones y las almejas dentro. Programe 18 minutos, temperatura Varoma, velocidad 1", "Retiramos ahora las valvas de los mejillones y las almejas (y reservamos los cuerpos) y con ayuda de un cestillo colamos el fumet y lo ponemos en un bol temporalmente", "Tras lavar bien el vaso y la tapa. Ponemos las zanahorias y el puerro troceado y lo programamos a velocidad 5 durante 4 segundos", "Después ncorpore el aceite y la sal. Programe 10 minutos, temperatura Varoma, giro a la izquierda, velocidad cuchara", "A continuación Añada la pulpa de pimiento choricero y el vino. Programe 5 minutos, temperatura Varoma, giro a la izquierda, velocidad cuchara.", "Incorpore el fumet reservado y programe 10 minutos, 100º, giro a la izquierda, velocidad cuchara. Cuando falten 4 minutos para terminar el tiempo programado, añada por el bocal el rape, los mejillones y las almejas.", "Por último cuando falte 1 minuto para terminar el tiempo programado, incorpore por el bocal los cuerpos de las gambas. Deje reposar un par de minutos. Vierta la sopa en la sopera y sirva inmediatamente." });
                break;
            case 8:
                //Peras roqueford
                //dificultad 2/5, 25 minutos
                recipe = new Recipe("Peras roqueford", 4, "raciones", "36", new int[] { 40, 2, 1, 50, 60, 0, 250 }, new string[] { "g. de avellanas tostadas", "peras", "zumo de medio limón", "g. de queso roqueford cortado en dados", "g. de nata", "una pizca de: tomillo seco, otra pizca de pimienta recién molida y un pellizco de sal", "g. de agua para el vapor" }
                    , 3, 25, "Thermomix"
                    , true, false, new string[] { "Introducimos en el thermomix las avellanas y lo troceamos a velocidad 5 durante 3 segundos. Lo retiramos en un bol y lo reservamos", "Corte las peras por la mitad transversalmente y, para evitar que se oxiden, póngalas en un bol con agua y el zumo de limón. Haga un hueco en el centro de cada mitad de pera retirando la pulpa con la ayuda de un sacabolas. Retire las pepitas y reserve la pulpa troceada.", " Ponga en el vaso el queso roquefort, la nata, el tomillo, la pimienta y la sal. Mezclamos a velocidad 4 durante 5 segundos.", "Agregue la pulpa de pera reservada y mezcle 1 segundo con velocidad 4", "Corte 4 rectángulos de papel de horno o de aluminio y coloque media pera en cada uno. Rellene los huecos de las peras con la mezcla. Cierre herméticamente cada papillote y colóquelos dentro del recipiente Varoma.", "Vierta el agua en el vaso, sitúe el Varoma en su posición y programe 15 min temperatura Varoma a velocidad 2. Abra los papillotes y sirva las peras espolvoreadas con los trozos de avellana" });
                break;
            case 9:
                //Salmonejo de espárragos
                //10 minutos, dificultad 1/5
                recipe = new Recipe("Salmonejo de espárragos", 6, "raciones", "37", new int[] { 80, 2, 2, 1, 1, 20, 50, 1 }, new string[] { "g. de aceite de oliva", "huevos duros", "dientes de ajo", "tarro de espárragos verdes escurridos", "tarro de espárragos blancos con su líquido (340 g.)", "g. de vinagre", "g. de pan", "1/2 cucharadita de sal" }
                    , 2, 10, "Thermomix"
                    , true, false, new string[] { "Coloque una jarra encima de la tapa del Thermomix, pulse la función balanza , pese el aceite y reserve.", "Pique un huevo duro: 2 segundos/velocidad 4. Lo Retiramos a un bol y lo reservamos", "Ponemos los ajos en el thermomis y troceamos a 5 segundos velocidad 5", "Corte algunas yemas (puntas) de los espárragos de los 2 colores y reserve para decorar el plato. Incorpore los espárragos blancos con su líquido, los espárragos verdes, el vinagre, el pan, la sal y el otro huevo duro y programe 20 segundos/velocidad 6.", "Seguidamente, programe 2 minutos/velocidad 5 2 minutos/velocidad 3.5 y, con el cubilete puesto, vaya añadiendo el aceite sobre la tapa para que caiga a hilo al vaso y emulsione. Vierta en un recipiente hermético y reserve en el frigorífico.", "Sirva frío decorado con las yemas de espárrago reservadas y acompañado con huevo duro picado." });
                break;
            case 10:
                //Solomillo de pavo con salsa de zanahoria
                //30 minutos 1/5
                recipe = new Recipe("Solomillo de pavo \n con salsa de zanahoria", 5, "raciones", "38", new int[] { 3, 100, 100, 300, 40, 75, 75, 1, 0, 0 }, new string[] { "solomillos de pavo", "g. de pimiento verde", "g. de pimiento rojo", "g. de zanahoria", "g. de aceite de oliva", "g. de vino blanco", "g. de agua", "pastilla de caldo de pollo", "un poco de tomillo", "hiervas provenzales" }
                    , 2, 30, "Thermomix"
                    , true, false, new string[] { "Primero lavamos las verduras y las vamos poniendo en el vaso de la thermomix a trozos. Trituramos 15 segundos a velocidad 5. No preocuparos por el tamaño, ya que, más tarde las trituraremos otra vez.", "Ahora bajamos la verdura de las paredes del vaso y añadimos el aceite. Programamos 10 minutos, temperatura varoma, giro a la izquierda, velocidad cuchara. Mientras cortamos a medallones los solomillos y los vamos colocando en las bandejas del recipiente Varoma.", "Terminado el tiempo, incorporamos el vino, agua, tomillo, las hierbas provenzales y la pastilla de caldo de pollo. Colocamos en el vaso el recipiente Varoma y programamos 20 minutos, temperatura varoma, giro a la izquierda, velocidad cuchara.", "Quitamos el recipiente Varoma y bajamos con la espátula, los que quede en las paredes del vaso. Trituramos 1 minuto, velocidad progresiva 5-10. Quedará una salsa un poco espesa, si os gusta más líquida, añadirle un poco de agua y volver a cocinar un par de minutos a temperatura varoma, velocidad 1." });
                break;
            case 11:
                //kingle de frambuesa
                //2 horas y 20 minutos, dificultad 3/5
                recipe = new Recipe("Kingle de frambuesa", 8, "raciones", "39", new int[] { 30, 4, 130, 30, 15, 1, 300 }, new string[] { "g. azúcar", "láminas de piel de limón", "g. de leche", "g. de levadura prensada fresca", "yema de huevo", "g. harina", "pellizco de sal" }
                    , 5, 160, "Thermomix"
                    , true, false, new string[] { "Ponga en el vaso el azúcar y la piel de limón y pulverice 15 segundos / velocidad 10. Con la espátula, baje los ingredientes hacia el fondo del vaso", "Añada la leche, la mantequilla y la levadura y mezcle 1 minutos / 37 grados / velocidad 2", "Añada la yema y mezcle 4 segundos /velocidad 3", "Incorpore la harina y la sal y amase 2 min. Retire la masa del vaso y dele forma de bola. Deje reposar dentro de una bolsa de plástico hasta que doble su volumen (aprox. 1 hora).", "Precaliente el horno a 180 grados. Forre una bandeja de horno con papel de hornear y reserve", "Ponga la masa sobre una superficie de trabajo espolvoreada con harina y extiéndala con el rodillo hasta tener un rectángulo de aproximadamente 40x50 cm.", "Unte la masa con la mermelada, espolvoree las nueces y enróllela por la parte más ancha. Corte el rollo por la mitad a lo largo, dejando el otro extremo sin cortar. Cruce la masa de forma que el corte quede hacia arriba y una los extremos formando una rosca", "Coloque la rosca en la bandeja preparada y hornee durante 20 minutos (180°C). Retire el kringle del horno y deje enfriar (30-40 minutos). Mientras tanto, mezcle en un bol la clara de huevo con el azúcar glas y vierta sobre el kringle sin cubrirlo totalmente. Sirva cuando se haya enfriado" });
                break;
        }
    }
    //10 recipes
    public void ChooseDrinksSP (int i)
    {
        //español
        switch (i)
        {
            //reciclado del 9 al 13 dieta sana
            case 1:
                //Batido tropical
                //muy fácil, 8 minutos
                recipe = new Recipe("Batido tropical", 2, "vasos grandes", "26", new int[] { 2, 4, 4, 4, 8 }, new string[] { "tazas de zumo", "trozos de mango", "trozos de piña", "trozos de melón", "cubitos de hielo" }
                        , 1, 8, "bebidas"
                        , true, false, new string[] { "Primero cortamos los trozos de mango, piña y melón.", "También exprimiermos las naranjas hasta conseguir la cantidad necesárea y picaremos también los cubitos de hielo", "Ponemos todo el la licuadora y lo liquamos hasta que no quede ningún grumo (si no tenemos licuadora también podemos usar una batidora). Finalmente le podemos añadir algo de azúcar" });
                break;
            case 2:
                //batido rojo con avena
                //muy fácil, 2 minutos
                recipe = new Recipe("Batido rojo avena", 2, "vasos grandes", "27", new int[] { 2, 4, 1 }, new string[] { "Tazas de leche de avena", "cucharadas de mezcla de frutos rojos (pueden ser frambuesa, mora, fresa, cereza ...)", "azúcar moreno" }
                        , 1, 2, "bebidas"
                        , true, false, new string[] { "Ponemos todos los ingredientes en la licuadora y los licuamos hasta que quede fina la mezcla y listo! (si no tenemos licuadora también podemos usar una batidora)" });
                break;
            case 3:
                //batido de plátano y fresa
                //muy fácil, 2 minutos
                recipe = new Recipe("Batido de fresa y plátano", 2, "vasos grandes", "28", new int[] { 8, 2, 2, 1 }, new string[] { "fresas", "plátanos", "tazas de leche de avellanas (puedes usar leche normal en su defecto)", "pizca de canela" }
                        , 1, 2, "bebidas"
                        , true, false, new string[] { "[Opcional] Previamente podemos poner la fruta en la nevera o congelador para que nos quede un batido más fresco y más rico", "Ponemos todos los ingredientes en la licuadora y los licuamos hasta que quede una masa uniforme y sin grumos (si no tenemos licuadora también podemos usar una batidora)" });
                break;
            case 4:
            //batido de papaya
            //muy fácil, 5 minutos
                recipe = new Recipe("Batido de papaya", 2, "vasos grandes", "29", new int[] { 1, 2, 2, 2 }, new string[] { "Papaya", "tazas de leche de avena", "pizcas de canela", "hielos" }
                    , 1, 5, "bebidas"
                    , true, false, new string[] { "Pelamos la papaya y la ponemos en la batidora o licuadora junto con la leche de avena y la canela. La trituramos bien hasta que no queden grumos" });
                break;
            case 5:
                //batido de té rojo
                //muy fácil 2 minutos
                recipe = new Recipe("Batido de té rojo", 2, "vasos grandes", "27", new int[] { 2, 1, 1, 1 }, new string[] { "tazas de té rojo", "taza de fresas o frambuesa", "cucharada de linaza o chia", "para endulzar podemos poner un poco de miel, azúcar o azúcar moreno" }
                        , 1, 2, "bebidas"
                        , true, false, new string[] { "Licuamos todo o lo mezclamos con una batidora si no tenemos licuadora hasta que nos quede una mezcla uniforme", "Vertemos la mezcla resultante en los vasos y los dejamos enfriar en la nevera" });
                break;
            case 6:
                //Cóctel de limón
                recipe = new Recipe("Cóctel de limón", 1, "vaso", "26", new int[] { 50, 25, 25 }, new string[] { "ml. fr ron blanco", "ml. Lima o limón si no tenemos", "ml. de sirope de azúcar" }
                        , 0, 0, "bebidas"
                        , true, false, new string[] { "Ponemos hielo en la copa de martini", "introducimos todos los ingredientes junto con los hielos en la coctelera o en un recipiente cerrado si no tenemos", "agitamos la coctelera y vertemos el líquido en la copa haciendo un doble filtrado", "podemos decorar el vaso con una rodaja de limón o azúcar en el borde" });
                break;
            case 7:
                //Chocolate mexicano de nuez y canela, 10 minutos, dificultad 2 / 5
                recipe = new Recipe("Chocolate mexicano de nuez y canela", 4, "raciones", "16", new int[] { 4, 3, 1, 1, 0, 0 }, new string[] { "tazas de leche", "rajas de canela", "1/2 taza de nueces pacanas, peladas y pulverizadas", "1 tablilla de chocolate (preferiblemente mexicano)", "canela en polvo para decorar", "opcionalmente ponemos añadir crema batida" }
                        , 0, 0, "bebidas"
                        , true, false, new string[] { "Primero mezclaremos la canela, las nueces y la tablilla de chocolate en una olla que calentaremos a fuego alto. Removeremos con una cuchara mientras se va calentando para mezclar los ingredientes", "Removeremos hasta que la leche comience a hervir, entonces bajaremos la temperatura hasta lograr un hervor suave durante 5 minutos", "Después dejamos reposar 10 minutos para que la nuez se cueza y le pase el sabor a la leche", "opcionalmente podemos colar los trozos de nuez si te molestan", "Finalmente lo servimos en tazas poniendo un poco de canela en polvo y nata si así lo deseamos" });
                break;
            case 8:
                //mojito casero, 10 minutos, dificultad 1/5
                recipe = new Recipe("Mojito casero", 2, "raciones", "15", new int[] { 6, 1, 0, 1, 1, 4 }, new string[] { "cucharadas soperas de azúcar moreno", "limón o lima exprimido", "hierbabuena o menta", "chorro de rón", "chorro de agua gaseosa", "hielos" }
                        , 0, 0, "bebidas"
                        , true, false, new string[] { "Cogemos el número de vasos anchos que necesitemos dependiendo de el número de mojitos que queramos hacer y exprimimos el zumo de medio limón o lima en cada uno. A continuación le pondremos tres cucharadas de azúcar moreno en cada vaso junto con varias hojas de menta si lo deseamos", "Machamos un poco el conjunto con un mortero o de manera manual", "Añadimos: primero los hielos, de segundo el ron y finalmente cubrimos la parte del vaso que queda libre con agua gaseosa. (Lo ideal sería añadir 2 dedos de alcohol)", "Finalmente removemos bien para que se mezcle todo y luego ya lo tendremos listo para servir!" });
                break;
            case 9:
                //Sangría 10 minutos dificultad 0/5, 100 calorías
                recipe = new Recipe("Sangría", 8, "raciones", "27", new int[] { 0, 0, 1, 1, 1, 1 }, new string[] { "2/3 de taza de zumo de limón", "1/3 de taza de zumo de naranja", "1/4 de taza de azúcar", "limón cortado en rebanadas delgadas", "naranja cortada en rebanadas delgadas", "botella de vino tinto" }
                        , 0, 0, "bebidas"
                        , true, false, new string[] { "en una jarra mezclaremos los zumos y el azúcar hasta que esté disuelta. A continuación añadiremos las rebanadas de limón y naranja a la jarra para que quede bonito y dé sabor", "Añadimos el vino a la mezcla y luego podemos ponerle hielos o dejarlo enfriar en la nevera si lo deseamos, pues lo tradicional es beberlo frío" });
                break;
            case 10:
                //zumo de remolacha, zanahoria y mandarina, 2 minutos 0/ 5
                recipe = new Recipe("zumo de remolacha, zanahoria y mandarina", 2, "porciones", "29", new int[] { 1, 1, 1, 0 }, new string[] { "taza de remolacha pelada y troceada", "taza de zanahoras peladas y troceadas", "tazas de zumo de mandarina (o naranja si no tenemos)", "opcionalmente podemos incluir hielo" }
                        , 0, 0, "bebidas"
                        , true, false, new string[] { "Colocamos todos los ingredientes en una licuadora o los batimos con una batidora si no tenemos. Y los licuamos o batimos hasta que no queden pedazos grandes", "A continuación lo colamos para que no queden tropezones", "Finalmente los servimos con hielo o lo ponemos en la nevera y dejamos que enfríe" });
                break;
            case 11:
                //zumo de remolacha, zanahoria y mandarina, 2 minutos 0/ 5
                recipe = new Recipe("Smoothie de Frutas", 2, "vasos", "26", new int[] { 2, 2, 2, 0 }, new string[] { "Melocotón", "kiwi", "plátano", "leche de soja" }
                        , 0, 2, "bebidas"
                        , true, false, new string[] { "Primero de todo pondremos todos los ingredientes en la licuadora", "licuamos durante 2 minutos hasta que quede una masa semi líquida típica de smoothies" });
                break;
        }
    }
    //2 recipes
    public void ChooseMeatsSP(int i)
    {
        //español
        switch (i)
        {
            case 1:
                //Costilla de cerdo con champiñones
                //dificil, 40 minutos
                recipe = new Recipe("Costilla de Cerdo con champiñones", 4, "personas", "41", new int[] { 1, 16, 75, 2, 0, 0, 0, 0 }, new string[] { "Kg de costilla de cerdo", "champiñones grandes", "g de mostaza antigua", "dientes de ajo", "un poco de tomillo", "aceite de oliva virgen extra", "sal", "perejil" }
                        , 4, 40, "carnes"
                        , true, false, new string[] { "Primero preparamos los champiñones quitándoles los tallos y labándolos a continuación los separamos en un bol", "pelamos y picamos los dientes de ajo para luego esparcirlos sobre los champiñones", " A continuacion añade una cucharada de tomillo, sazona y riégalos con un chorrito de aceite. Deja que maceren durante unos 10 minutos", "Cortaremos la costilla en trozos para ponerlos en la olla rápida. Cubrimos con agua, sazona y añadimos unas ramas de perejil. las conemos durante unos 10 minutos. Retiramos, escurrimos y las dejamos templar.", "Pon a calentar una sartén. Añade los champiñones con el líquido del macerado, tapa y cocínalos por los 2 lados durante unos 6-8 minutos. Espolvoréalos con perejil picado.", "Unta las costillas con la mostaza antigua y ásalas en la barbacoa durante 3- 4 minutos por cada lado. Luego las servimos en el plato ¡y listo!" });
                break;
            case 2:
                //Alitas de pollo al horno
                //fácil, 40 minutos
                recipe = new Recipe("Alitas de pollo asadas al horno", 2, "personas", "42", new int[] { 10, 1, 3, 1, 2, 0 }, new string[] { "alas de pollo", "cucharada de miel", "cucharadas de salsa de soja", "cucharadita de pimentón", "cucharadas de aceite de oliva", "sal" }
                        , 2, 40, "carnes"
                        , true, false, new string[] { "Primero cortaremos las alitas en 3, siguiendo la articulación y elimina la punta que carece de carne. Con un cuchillo retírales las partes sobrantes de piel (lugar en el que se concentra toda la grasa) y extiéndelas sobre la placa de horno.", "Pon en un cuenco la miel, la salsa de soja, el pimentón y las cucharadas de aceite. Bate todo bien con un tenedor hasta que quede bien unificado.", "Unta las alitas con la mezcla anterior, sálalas e introduce la bandeja en el horno (previamente calentado durante 10 minutos) a 190 grados durante 25 minutos. Retíralas y sirve." });
                break;
            case 3:
                //Alitas de pollo al horno
                //fácil, 40 minutos
                recipe = new Recipe("Raxo con patatas y pimientos de padrón", 4, "personas", "43", new int[] { 1, 3, 0, 0, 0, 500, 250 }, new string[] { "kg de cinta de lomo de cerdo", "dientes de ajo", "sal fina", "aceite de oliva virgen extra", "pimienta negra", "g de patatas gallegas", "g de pimientos de padrón" }
                        , 2, 40, "carnes"
                        , true, false, new string[] { "El raxo es lomo de cerdo en trozos, por lo que lo ideal es cortar la carne justo cuando vayamos a cocinar la receta. Si vais a la carnicería el mismo día, podéis pedirles que os troceen la carne y así ahorráis trabajo luego en casa. Si no es el caso, pedid una pieza entera.", "Retiramos a la carne los excesos de grasa, en la parte exterior. La cortamos en lonchas de 3 cm. y luego estas en dados (3 cm. x 3 cm.). Hacemos este proceso con toda la pieza.", "Picamos el ajo finito. En un bol, echamos la carne y adobamos con el ajo, sal, pimienta negra (recién molida si puedes ser) y un chorro de aceite. En algunos sitios de Galicia encontraréis también el raxo con un buen chorro de vino blanco y un toque de orégano. Aunque en la receta de hoy hemos optado por la receta más general.", "Removemos todo muy bien con una cuchara de madera, para integrar bien los ingredientes y que la carne pille todo el sabor.", "Lo tapamos todo con un trapo o la tapadera del bol para que la nevera no pille olores. Lo dejamos reposando en la nevera durante un día entero.", "Cuando queden 10 minutos para cumplirse el tiempo, vamos a preparar las patatas. Las pelamos y las cortamos al estilo tradicional, a lo largo. Las ponemos a freír en aceite de oliva virgen extra." });

                break;
        }
    }

    //ENGLISH RECIPES
    //12 recipes //NOT TRANSLATED YET
    public void ChooseDessertEN (int i)
    {
        //titulo, cantidad, palabra de cantidad
        //img, int ingredientes, string ingredientes
        //dificultad, tiempo de preparación, pasos a seguir
        switch (i)
        {
            case 1:
                //Tarta de queso y chocolate
                recipe = new Recipe("Chocolate Cheesecake", 6, "raciones", "10", new int[] { 350, 150, 400, 150, 20 }, new string[] { "ml de nata para montar al 35%", "g de azúcar", "g de queso Philadelphia", "g de chocolate de cobertura al 54%", "cm sería la longitud del molde redondo que necesitaríamos" }
                                            , 3, 160, "postres"
                                            , true, false, new string[] { "Lo primero sería preparar la salsa de chocolate. Lo ponemos en el microondas en intervalos de 30 segundos y lo removemos hasta que se vuelva líquido. A continuación añadimos 150 ml de nata y dejamos reposar 1 minuto hasta que se convierta en una crema", "(opcional) si queremos hacer un bizcocho para la base deberías hacerlo ahora para que dé tiempo a enfriar", "En un bol incluímos el queso y el azúcar y removemos bien hasta que obtengas una crema, finalmente añadimos el resto de la nata y mezclamos bien", " Colocamos la mitad de la crema en el molde desmontable (encima del bizcocho si lo tenemos). La extendemos y la ponemos encima de la nevera durante 15 minutos. A continuación echamos el resto de la crema y el resto de la salsa de chocolate", "Lo ponemos en la nevera un mí­nimo de 2 horas y desmoldamos al servir" });

                break;
            case 2:
                //cupcakes de chocolate (14 unidades)							
                recipe = new Recipe("Chocolate Cupcakes", 14, "unidades", "11", new int[] { 3, 1, 150, 100, 60, 135, 16, 50, 14, 300, 4, 3 }, new string[] { "huevos medianos", "yogur", "g. de azúcar", "ml de aceite de olvia", "g. de cacao sin azúcar", "g. harina de reposterí­a", "g. de levadura en polvo", "g. de chocolate negro", "g. de queso crema", "cucharadas de azúcar", "cucharadas de nata líquida", "g. de chocolate negro" }
                                            , 1, 30, "postres"
                                            , true, false, new string[] { "En un bol incluímos los huevos, el azúcar y lo batimos hasta que hagan espuma", "Mientras seguimos batiendo echamos el yogur y el aceite. También vamos incorporando el caco y removemos bien. Luego incluimos la harina en la que incluiremos una cucharadita de levadura y mezclamos bien", "Con la masa resultante echaremos 2 cucharadas en cada molde (dejando la mitad del molde sin cubrir para que salga mejor)", "A continuación podemos ponerlas en el microondas (a 3 minutos 6 unidades) o podemos hornearlas a 180 grados entre 20 y 25 minutos", "Ahora nos falta hacer la crema de chocolate: derretimos el chocolate y lo ponemos al microondas en intervalos de 30 segundos, después removemos hasta que quede líquido", "mezclamos bien el queso crema con la nata y el azúcar hasta que se vuelva con brillo, finalmente incluímos el chocolate derretido y removemos bien", "Decoramos colocando la crema en una bolsa o manga pastelera desechable colocamos la boquilla con forma de churro, cortamos en la punta y dibujamos de fuera hacia adentro. Salpicamos unas virutas de chocolate o chocolate derretido y un trozo de chocolate para decorar." });

                break;
            case 3:
                //tarta de huesitos de 6 raciones							
                recipe = new Recipe("Huesitos Cake", 6, "raciones", "12", new int[] { 12, 250, 150, 50, 10, 6 }, new string[] { "huesitos", "g. de chocolate", "g. de nata o crema de leche", "g. de mermelada de fresa", "nuves", "golosinas" }
                                            , 4, 20, "postres"
                                            , true, false, new string[] { "Lo primero será fundir 150 gramos de chocolate, puede ser al baño marí­a o al microondas. Si es en el microondas lo pondremos a intervalos de 30 segundos y lo revolveremos suavemente hasta que se funda", "A continuación, ponemos 100ml de nata al fuego y mientras se calienta vamos troceandos los otros 100 g. de chocolate en trozos pequeños.", "Cuando la nata rompa a hervir la retiraremos del fuego, le echaremos el chocolate y removeremos hasta que se funda. Finalmente le echaremos un par de cucharadas de mermelada de fresa y mezclamos hasta que se mezclen bien los ingredientes", "Para montar nuestra tarta la colocaremos sobre una fuente plana y haremos una base con el chocolate fundido del ancho y largo de 6 guesitos juntos uno a par del otro", "Una vez que tenemos la base colocamos los huesitos juntos. Para que se unan mojaremos uno de los lados con chocolate y presionamos con el anterior huesito", "Una vez que tenemos el primer piso de huesitos hacemos un borde por la parte exterior con el chocolate fundido y rellenamos el interior con la ganache de fresa. En este punto decidiremos si queremos darle un extra de grosor de ganache (cosa que yo recomiendo pues está buení­sima) en cuyo caso lo meterí­amos unos 5 minutos en el frigorífico", "Pasado este tiempo volvemos a echar otro borde de chocolate y a rellenar el centro con la ganache. Si no queremos este grosor extra nos saltamos este paso.", "A continuación colocamos un segundo piso de huesitos de la misma forma. Una vez finalizado el segundo piso echamos una capa de chocolate fundido por encima y la enfriaremos en el frigorífico durante 10 minutos hasta que endurezca el chocolate", "Finalmente echaremos el resto de la nata en el chocolate fundido que nos ha sobrado y, como está frí­o lo calentaremos unos 30 segundos en el microondas y removemos hasta que quede una masa fluída.", "Ahora cortamos las nubes hasta conseguir unos discos parecidos a unos labios. Quitamos la tarta del frigorífico, la soltamos del papel y cortamos los bordes que pudiesen quedar. Con ayuda de una espátula la colocamos en la fuente donde la vamos a presentar y colocamos por encima una capa de labios de nube.", "Repartimos la última ganache sobre las nubes hasta que queden cubiertas, sin preocuparnos que pueda caer algo por los bordes, eso le dará un toque de contraste muy apetecible. Servimos o reservamos en un lugar fresco. Acompañaremos con un bol donde pondremos el resto de nubes y gominolas y otro con el resto de la ganache de fresa." });
                break;
            case 4:
                //9 Tortitas caseras
                recipe = new Recipe("Homemade Pancakes", 9, "unidades", "13", new int[] { 2, 3, 1, 425, 400, 20, 50 }, new string[] { "huevos", "cucharadas de azÃºcar", "pizca de sal", "ml de leche", "harina fina de reposterÃ­a", "g. de levadura en polvo (2 sobres)", "g. de mantequilla" }
                                            , 1, 10, "postres"
                                            , true, false, new string[] { "Para hacer la masa pondremos en un bol: los huevos el azúcar y la sal. Lo batimos enérgicamente y añadiremos casi toda la leche (dejaremos 100 ml) y lo mezclamos todo de nuevo", "Lo pasamos a una jarra. Luego en un bol tamizamos (pasamos por un colador) la harina junto con la levadura en polvo, hacemos un hueco en el centro y vamos echando la mezcla liquida y removiendo con una cuchara del centro hacia afuera para que no se creen brumos.", " De esta manera no se crean grumos si vemos que está demasiado líquida nunca añadiremos la harina en seco, por eso preferimos echar poco liquido y cuando la mezcla esté echa le vamos añadiendo leche hasta que esté en el punto de espesor.", "Ahora ponemos un poco de mantequilla en una sartén de unos 24 cms a fuego medio bajo y cuando esté caliente echamos un tamaño de 3 cucharadas de la masa. Cuando lleve un minuto aproximadamente le damos la vuelta (cuando coja color) dejamos otro minuto aproximadamente y vamos colocando en un plato. ", "Para decorarlo: podemos acompañarlo con nata liquida montada, sirope de fresas, chocolate o caramelo, o en su lugar un chorro de miel, unas frutas en taquitos y listo!" });
                break;
            case 5:
                // Volcán de chocolate en un minuto (para 4 personas)
                recipe = new Recipe("Chololate volcano", 4, "personas", "14", new int[] { 125, 50, 3, 80, 1, 1 }, new string[] { "g. de cobertura de chocolate", "g. de mantequilla", "huevos", "g. de azúcar", "cucharada de harina de reposterí­a", "copita de licor de naranja" }
                                            , 2, 5, "postres"
                                            , true, false, new string[] { "Fundimos el chocolate con la mantequilla en intervalos de 30 segundos y mezclamos bien hasta que quede uniforme", "en un bol incluimos los huevos, azúcar y batimos hasta que quede espumita y quede de color amarillo claro. A continuación añadimos la mezcla de chocolate y removemos una vez más hasta conseguir una mezcla uniforme.", "Finalmente añadimos una cucharada de harina pasándola por un colador (tamizar) y le añadiremos también el licor de naranja", "Lo repartimos en diferentes cuencos dependiendo de el número de personas que lo vallan a comer. Lo ponemos al microondas 1 minuto . Lo decoramos a nuestro gusto y servimos caliente." });
                break;
            case 6:
                // Mousse de limón: para 4
                recipe = new Recipe("Lemon Mousse", 4, "raciones", "15", new int[] { 125, 300, 4, 1, 125, 350 }, new string[] { "ml de zumo de limón", "g. de azúcar", "yemas de huevo", "cuchara de rayadura de limón", "g. de mantequilla sin sal", "ml de nata lí­quida para montar" }
                                            , 2, 250, "postres"
                                            , true, false, new string[] { "Primero de todo tendremos que preparar el almíbar de limón: Para ello, Ponemos a calentar en un cazo 125 ml (media taza) de zumo de limón, 125 g de azúcar Hervimos a fuego lento. Mientras dejamos que se tiemple un poco montamos la nata (crema de leche), aÃ±adiendo 4 cucharadas de azúcar casi al final.", "A continuación prepararemos el mousse: Ponemos 4 yemas de huevo en un bol, añadimos 125 g. de azúcar. Vamos incorporando poco a poco el almíbar de limón y mezclandolo con las yemas, removiendo continuamente. Volvemos a echar la mezcla en el cazo y cocemos hasta que salgan burbujas.", "Lo dejamos enfriar 10 minutos y luego batimos hasta que duplique su volumen. Añadimos la ralladura de limón (1 cucharada) y 125 g de mantequilla sin sal a temperatura ambiente cortada en trozos. Añadimos los 350 g ( 1 y  1/2 taza ) de nata ya montada y mezclamos suavemente con la espátula.", "Dejamos enfriar 4 horas para que cambie su sabor y espese creandose el aspecto de mousse espumoso (como queso), listo!" });
                break;
            case 7:
                // Mousse de chocolate para 4
                recipe = new Recipe("Chocolate Mousse", 4, "personas", "16", new int[] { 175, 30, 4, 80, 1 }, new string[] { "g. de cobertura de chocolate", "g. de mantequilla", "huevos", "g. de azúcar", "(opcional) una copa de ron, brandy o ralladura de naranja" }
                                            , 2, 130, "postres"
                                            , true, false, new string[] { "Primero de todo fundimos el chocolate y la mantequilla en el microondas en intervalos de 30 segundos y lo mezclaremos hasta que quede uniforme. También podemos hacerlo al baño maría", "Separamos las claras de las yemas. En el bol de las yemas añadimos el azúcar y vamos removiendo con ayuda de una espátula añadiendo la mezcla de chocolate a cucharadas poco a poco, hasta que consigamos una mezcla uniforme.", "Batimos las claras a punto de nieve y las vamos incorporando poco a poco a la anterior mezcla de chocolate. Vamos mezclando despacio con movimientos envolventes hasta conseguir que quede uniforme la mezcla. No debemos utilizar batidora. Si no lo van a tomar los niños podemos añadirle una cucharada de licor tipo coñac o ron.", "Lo repartimos en el número de cuencos que queramos y lo dejamos enfriar en la nevera para que se integre durante un par de horas" });
                break;
            case 8:
                //Bizcocho casero para 8 personas
                recipe = new Recipe("Homemade Biscuit", 8, "personas", "17", new int[] { 7, 1, 80, 160, 140, 1, 25, 200 }, new string[] { "huevos", "yogurt natural", "g. aceite (preferiblemente de oliva)", "g. de azúcar", "g. de harina de repostería", "sobre con levadura química", "ralladura de naranja", "uvas blancas", "g. de chocolate" }
                                            , 4, 50, "postres"
                                            , true, false, new string[] { "Precalentamos el horno a 180 grados y preparamos un molde (aconsejable 22 cm de diámetro)", "separamos en recipientes distintos las yemas de las claras y batimos las claras a punto de nieve", "A continuación batimos las yemas con el azúcar hasta formar una crema, añadimos el yogurt y batimos, el aceite y batimos, a partir de este momento apagamos la batidora eléctrica.", "A continuación mezclamos la harina con la levadura y la pasamos por un colador incorporando a la mezcla y removiendo con una espátula o cuchara. Echamos la mitad de la clara a punto de nieve y mezclamos suavemente.", "Añadimos el resto y terminamos de mezclar con suaves movimientos envolventes para que no baje la mezcla. Vertemos en el molde y dejamos caer las uvas peladas sin pepita repartidas por la mezcla. Horneamos a 180 grados durante 30 o 35 minutos.", "Cuando saquemos del horno el bizcocho lo dejamos enfriar durante unos 30 minutos que reposará en el molde. Una vez frí­a quedará prácticamente suelta de los bordes y sólo habrí­a que desmoldarla, quitarle el papel de hornear y presentarlo en la fuente.", "Entonces fundimos los 200 g de chocolate de cobertura al baño marí­a o en el microondas 30 seg, removemos bien y echamos por encima del bizcocho, llegados a este momento pasamos por encima un tenedor haciendo un dibujo de ondas (como el tronco de navidad) y dejamos enfriar. Colocamos las virutas encima y enfriamos" });
                break;
            case 9:
                //Galletas de almendra (para 4 personas)
                recipe = new Recipe("Almond cookies", 4, "personas", "18", new int[] { 120, 100, 50, 1, 300, 16 }, new string[] { "g. de mantequilla", "g. de azúcar moreno (puede ser azúcar blanco si no tenemos)", "huevo", "g. de harina de repostería", "g. de levadura", "almendras en láminas" }
                                            , 3, 90, "postres"
                                            , true, true, new string[] { "En un bol metemos el huevo y lo batimos bien, a continuación añadiremos la mantequilla (la calentamos un poco si es necesario para que no quede espesa) y los dos tipos de azúcar para mezclarlo todo", "Ahora sobre una fuente colocamos un papel de hornear ponemos la masa en el centro y colocamos encima otro papel de hornear. Con ayuda de un rodillo extendemos la masa dejando un grosor aproximado de 3 monedas. Dejamos enfriar en el frigorífico durante una hora.", "Mientras tanto precalentamos el horno a 160ºC, quitamos el papel de la parte superior y lo colocamos como base en la bandeja en la que vamos a hornear. Vamos cortando la masa con un cortapastas o un vaso y colocamos las galletas en la fuente en la que vamos a hornear.", "Juntamos los recortes que nos quedan de masa y la volvemos a extender para hacer mas galletas hasta que se acabe.", "Finalmente repartimos las láminas de almendra por encima de las galletas presionando ligeramente. Horneamos a 160ºC durante 15-20 minutos hasta que estén doradas. retiramos dejamos enfriar y servimos" });
                break;
            case 10:
                //Gominolas caseras
                recipe = new Recipe("Homemade candies", 6, "Personas", "19", new int[] { 200, 300, 2, 1, 1, 1 }, new string[] { "ml de agua (se puede cambiar por zumo o leche)", "g. de azúcar", "sobres de gelatina neutra", "sobres de gelatina sabor fresa", "poco de azúcar para el rebozado", "poco de aceite" }
                                            , 1, 250, "postres"
                                            , false, false, new string[] { "Primero pintamos los moldes de silicona o el recipiente donde pondremos las gominolas con aceite", "A continuación calentamos en un cazo el agua (zumo o leche) y añadiremos también los dos sobres de gelatina neutra y removemos. Cuando este diluída incluiremos también la gelatina de sabor y el azúcar. Finalmente lo dejaremos al fuego unos 4 minutos sin dejar de remover", "Sin dejarlo enfriar vertemos el líquido en los moldes de silicona.  Debemos esperar unas 6 horas hasta que las gominas se hayan enfriado. Si tenemos un poco de prisa se pueden dejar una hora menos  en la nevera.", "Finalmente desmontamos en gominolas y rebozamos en el azÃºcar" });
                break;
            case 11:
                //Leche frita
                recipe = new Recipe("Fried milk", 6, "raciones", "20", new int[] { 1, 1, 3, 1, 80, 50, 1 }, new string[] { "medio litro de leche", "medio palo de vainilla o monda de limón", "yemas de huevo", "huevo", "g. de azúcar", "g. de harina", "pizca de mantequilla para untar por encima" }
                                            , 2, 10, "postres"
                                            , true, false, new string[] { "Primero herviremos la leche con los aromas que le van a dar sabor, en este caso serí­a la vainilla pero podemos usar por ejemplo una monda de limón. Cuando hierva lo dejaremos templar", "En un bol aparte ponemos el azúcar con la harina removemos y echamos un poco de leche templada y añadimos las yemas y el huevo batidos y removemos bien.", "Agrega a esta mezcla al resto de la leche y calentamos a fuego medio hasta que espese sin parar de remover.", "Enfríalo en un recipiente y coloca la mantequilla como una capa por encima." });
                break;
            case 12:
                //Flan de huevo casero
                recipe = new Recipe("Flan (Spanish Pudding)", 3, "raciones", "21", new int[] { 4, 4, 400, 1 }, new string[] { "huevos", "cucharadas colmadas de azúcar", "ml de nata líquida (o crema de leche)", "poco de canela y limon (también podéis usar naranja coñac o whisky)" }
                                            , 2, 35, "postres"
                                            , true, false, new string[] { "Incluiremos los ingredientes por el siguiente orden: huevos, azúcar, leche, canela y limón y vamos removiendo.", "En otra tartera (que quepa en otra olla más grande) se hace caramelo con 10 cucharadas de azúcar y 1/2 chupito de agua para que moje el azúcar (no remover con la cuchara ). Se deja hacer hasta que adquiera su color tostado y espese.", "Una vez está el caramelo se echa lo batido, y este recipiente se pone dentro de la tartera  grande en la que previamente se echó agua suficiente para que quede por encima del nivel del flan. Se deja estar entre 25 y 30  minutos." });
                break;

        }

        return;

    }
    //11 recipes
    public void ChooseAppetizerEN (int i)
    {
        //inglés
        switch (i)
        {
            case 1:
                //Almejas al ajillo
                recipe = new Recipe("Clams with garlic", 2, "tapas", "40", new int[] { 20, 2, 1, 1, 1, 3 }, new string[] { "Clams", "Cloves of garlic", "Some chopped parsley to decorate", "Some extra virgin olive oil", "Chilli pepper to taste", "Salt" }
                        , 1, 15, "aperitivo"
                        , true, false, new string[] { "We leave the clams in the water with water during the hours, so that they eliminate the sand that they may have inside.", "Next, we laminate the cloves of garlic and cut a piece of chilli pepper and put them in an infuser in the tempered oil, with the fire to the minimum to avoid turning the garlic very quickly.", "Add the clams and cover the pan so that the steam helps the clams to open.", "While they are all open, we are going to chop parsley, which we will use at the last moment to sprinkle on the clams with garlic, which will be finished and friends ready for the tasting!" });
                break;
            case 2:
                //Homemade waffles: between 6 and 8 units
                recipe = new Recipe("Homemade Wafles", 7, "units", "0", new int[] { 150, 8, 30, 1, 200, 40 }, new string[] { "g. of baking flour", "g. of baking powder", "g. of sugar", "eggs", "ml of cream", "g. of butter" }
                        , 3, 35, "aperitivo"
                        , true, false, new string[] { "We have to separate the white and the yolk in 2 different recipients, after that we will churn the white", "To the yolk we add yeast mixed with flour, cream (or milk if we do not have), sugar and butter. We will need to mix everything well and let it rest for 20 to 30 minutes", "We heat the waffle and pour the necessary amount of our mixture. We close the waffle and give the cooking time you need, in our case between 4 and 5 minutes", "Done! now we can put on top of them chololate, mermelade, cream or whatever we like!" });
                break;

            case 3:
                //pastél japonés para 4 personas
                recipe = new Recipe("Japanese cake", 4, "persons", "1", new int[] { 3, 120, 120, 1 }, new string[] { "eggs", "g. of white chocolate", "g. of creamy cheese", "little bit of powdered sugar for decoration" }
                        , 3, 45, "aperitivo"
                        , true, false, new string[] { "First of all we preheat the oven to 170 degrees and we will separate the yolks from the whites", "We put the white chocolate in a bowl and we melt it in the microwave with intervals of 15 seconds. After that we remove it until it gets undone. Then we add the cheese in cream, we need to stir well and finally add the yolks and then, we will mix all the ingredients once more", "Now we will mix the whites energically and we will add them to the mix", "We put the mixture in a mold which size should be around 20 cms and we will put it on the tray of the oven in which we have poured water. Leave the cake in the oven at 170 degrees for 15 minutes.", "After that time we lowered the oven temperature to 160 degrees and left another 15 minutes. Finally, we turned off the oven but do not remove the cake because we left another 15 minutes. When we emulate it we sprinkle with sugar glass and let it cool" });
                break;

            case 4:
                //canapés tropicales 4 raciones
                recipe = new Recipe("tropical canapes", 4, "portions", "2", new int[] { 1, 1, 1, 1, 1, 1, 8, 1, 1, 4 }, new string[] { "avocado", "mango", "Pot of red beans", "pepper", "chilli", "spring onion", "Wheat tortillas and corn", "lima", "a little bit of oil", "Glasses or molds to bake the fajitas" }
                        , 2, 5, "aperitivo"
                        , true, false, new string[] { "First we cut the fajitas in 4 parts and place them in the molds. Then we press the middle part so that they form a kind of baskets. With a brush or kitchen brush, we wet the inside of the Tortillas a bit. Then we preheat the oven to 180ºC and bake the fajitas in the molds for 8 minutes.", "Cortamos el pimiento, la cebolleta y la Guindilla en cuadrados pequeños. Partimos el aguacate a la mitad y le quitamos el corazón; cortamos en cuadrados sin llegar a la piel y le quitamos la piel. Hacemos lo mismo con el Mango.", "In a large bowl we put the pot of red drained beans, the pepper, the chives, the chilli, the avocado and the mango. Squeeze a file above the ingredients and mix everything well. Fill our baskets with ingredients and you are finished!" });
                break;

            case 5:
                //patatas asadas al microondas en solo 3 minutos! (2 personas)
                recipe = new Recipe("Microwave roasted potatoes", 2, "persons", "3", new int[] { 4, 1, 1, 1 }, new string[] { "small potatoes", "jet of olive oil", "Pinch of black pepper", "foil" }
                        , 1, 5, "aperitivo"
                        , true, false, new string[] { "First we will spread the foil in the center and put a trickle of oil. Place a few small potatoes and sprinkle some with black pepper. Finally we take another trickle of oil", "Wrap so that the steam does not come out. Put the opening of the paper face down in the dish or fountain where we go to cook.", "We put it in the microwave at medium temperature (approx 650 watts) for 3 minutes, take out, turn around and put half a minute more." });
                break;

            case 6:
                //Ensalada de tomate
                recipe = new Recipe("Tomato salad", 2, "small portions", "4", new int[] { 3, 1, 1, 1, 1 }, new string[] { "olive oil spoons", "1/4 cup of balsamic vinegar", "box of washed tomatoes", "1/4 Cup of basil leaves", "bit of thick salt", "Some freshly ground pepper" }
                        , 1, 3, "aperitivos"
                        , true, false, new string[] { "First we heat the vinegar and the olive oil until it begins to make bubbles (it usually takes about 2 minutes approximately)", "Then we will mix with the freshly heated liquid: the salt, the pepper and the basil and let it rest before serving", "Just before serving we must take the rest of the vinegar and sprinkle the salad with it, finally we will add a pinch of coarse salt" });
                break;

            case 7:
                //Ensalada caprese
                recipe = new Recipe("Carpese Salad", 2, "small portions", "5", new int[] { 1, 3, 1, 1, 1 }, new string[] { "Some fresh mozzarella", "mature tomatoes", "Bunch of basil", "Some olive oil", "Some sea salt black pepper" }
                        , 1, 2, "aperitivos"
                        , true, false, new string[] { "First of all we will cut the tomatoes in thin slices by placing them on a tray one by one", "Next we will put on each slice of tomato a leaf of basil and a layer of mozzarella", "Finally we sprinkle the salad with olive oil and enrich it with a little salt and black pepper so that when we eat it is 100% fresh" });
                break;

            case 8:
                //Tomate y ensalada de maíz
                recipe = new Recipe("Tomato and corn salad", 2, "Small portions", "4", new int[] { 1, 4, 1, 1, 1, 1 }, new string[] { "Some fresh and sweet corn", "Mature tomatoes", " A 1/4 cup of fresh mint leaves", "A 1/4 Cup fresh herbs (can be parsley, basil, rosemary, sage and a tablespoon of olive oil)", "Little salt and fresh black pepper", "Little fresh goat cheese" }
                        , 2, 6, "aperitivos"
                        , true, false, new string[] { "First of all you must put water in a large pot, and when it reaches the boil add the corn and let it boil for 5 minutes", "While the corn is boiling, we will cut the tomatoes into quarters or slices. Scrape or squeeze the juice and remove the remaining moisture with a bit of paper", "Once the corn is cooled we will put each cob standing on top of a wide, shallow bowl. Finally we will mix the corn kernels with a sharp and shallow knife to mix the corn and the tomatoes", "Finally we will add the mint leaves, the herbs, the olive oil, the salt and the pepper." });
                break;
            case 9:
                //Ensalada de patata
                recipe = new Recipe("Potato salad", 6, "portions", "6", new int[] { 1, 2, 6, 15, 100, 1, 1, 1, 1, 1, 1 }, new string[] { "Kg. Of Mediterranean baby potatoes, cooked and peeled", "Small spoonfuls of roasted cumin seeds", "Red onions for salad", "G. of fresh mint", "g. de grenade", "1/4 of cup of oil", "1/2 Cup of mayonnaise", "Pinch of saffron in a little hot water", " half a lemon", "a little bit of salt", "Little black pepper" }
                        , 2, 10, "aperitivos"
                        , true, false, new string[] { "Cut the potatoes in half and place in a bowl after adding red onions and cumin seeds", "We mix the ingredients of the dressing and we pour them on the potatoes so that it takes flavor", "We continue decorating with fresh mint leaves and pomegranate seeds" });
                break;
            case 10:
                //pincho de salmón
                //dificultad 1 y tiempo de preparación 3 minutos
                recipe = new Recipe("Pincho of salmon", 4, "raciones", "7", new int[] { 4, 4, 1, 1, 1 }, new string[] { "Bread toasts", "Lettuce leaves", "A bit smoked salmon", "A bit of shrimps cooked and peeled", "a bit of mayonnaise" }
                        , 1, 2, "aperitivos"
                        , true, false, new string[] { "First we cut the lettuce in small pieces and we put it in a bowl, in addition we will add the salmon and the prawns in the same one", "Finally we mixed it well and we put the mixture on the toasts of bread" });
                break;
            case 11:
                //pincho de gambas empanadas
                //dificultad 2 tiempo de preparación 25 minutos
                recipe = new Recipe("Pincho of \n breaded shrimps", 12, "small portions", "8", new int[] { 12, 1, 1, 1, 1, 1, 1 }, new string[] { "shrimps", "bit of salt", "bit of white pepper", "wheat flour", "backed egg", "a bit of breadcrumbs", "a bit of frying oil" }
                        , 2, 5, "aperitivos"
                        , false, true, new string[] { "The first thing is to peel the shrimp by removing their head and skin. Then add salt and pepper", "We punctured them in wooden skewers and removed them. First with flour and then with beaten egg and finally with breadcrumbs", "Then we fry them in oil, and our food will be ready!" });
                break;
            case 12:
                //pinchos de jamón tomate y queso
                //dificultad 1, tiempo 3 minutos
                recipe = new Recipe("Pincho of Ham, cheese \n and cherry tomato", 8, "units", "9", new int[] { 30, 30, 30, 1, 1 }, new string[] { "Cheese squares (approx)", "Squares of cooked ham (approx)", "cherry tomatoes", "Iceberg lettuce", "Wooden or colored sticks" }
                        , 1, 3, "aperitivos"
                        , true, false, new string[] { "First of all we will have to wash the tomatoes and lettuce leaves, then let them dry", "In each of the sticks we will insert: 1 square of cheese, 1 square of ham and a cherry tomato", "For the decoration we will put in the middle of the plate the leaf of lettuce and a few cherry tomatoes on the lettuce", "Finally put the skewers around the lettuce." });
                break;
        }

    }
    //13 recipes
    public void ChooseDietEN (int i)
    {
        //inglés
        switch (i)
        {
            case 1:
                //Ensalada de tomate
                recipe = new Recipe("Tomato salad", 2, "small portions", "4", new int[] { 3, 1, 1, 1, 1 }, new string[] { "olive oil spoons", "1/4 cup of balsamic vinegar", "box of washed tomatoes", "1/4 Cup of basil leaves", "bit of thick salt", "Some freshly ground pepper" }
                        , 1, 3, "aperitivos"
                        , true, false, new string[] { "First we heat the vinegar and the olive oil until it begins to make bubbles (it usually takes about 2 minutes approximately)", "Then we will mix with the freshly heated liquid: the salt, the pepper and the basil and let it rest before serving", "Just before serving we must take the rest of the vinegar and sprinkle the salad with it, finally we will add a pinch of coarse salt" });
                break;

            case 2:
                //Ensalada caprese
                recipe = new Recipe("Carpese Salad", 2, "small portions", "5", new int[] { 1, 3, 1, 1, 1 }, new string[] { "Some fresh mozzarella", "mature tomatoes", "Bunch of basil", "Some olive oil", "Some sea salt black pepper" }
                        , 1, 2, "aperitivos"
                        , true, false, new string[] { "First of all we will cut the tomatoes in thin slices by placing them on a tray one by one", "Next we will put on each slice of tomato a leaf of basil and a layer of mozzarella", "Finally we sprinkle the salad with olive oil and enrich it with a little salt and black pepper so that when we eat it is 100% fresh" });
                break;

            case 3:
                //Tomate y ensalada de maíz
                recipe = new Recipe("Tomato and corn salad", 2, "Small portions", "4", new int[] { 1, 4, 1, 1, 1, 1 }, new string[] { "Some fresh and sweet corn", "Mature tomatoes", " A 1/4 cup of fresh mint leaves", "A 1/4 Cup fresh herbs (can be parsley, basil, rosemary, sage and a tablespoon of olive oil)", "Little salt and fresh black pepper", "Little fresh goat cheese" }
                        , 2, 6, "aperitivos"
                        , true, false, new string[] { "First of all you must put water in a large pot, and when it reaches the boil add the corn and let it boil for 5 minutes", "While the corn is boiling, we will cut the tomatoes into quarters or slices. Scrape or squeeze the juice and remove the remaining moisture with a bit of paper", "Once the corn is cooled we will put each cob standing on top of a wide, shallow bowl. Finally we will mix the corn kernels with a sharp and shallow knife to mix the corn and the tomatoes", "Finally we will add the mint leaves, the herbs, the olive oil, the salt and the pepper." });
                break;
            case 4:
                //Ensalada de patata
                recipe = new Recipe("Potato salad", 6, "portions", "6", new int[] { 1, 2, 6, 15, 100, 1, 1, 1, 1, 1, 1 }, new string[] { "Kg. Of Mediterranean baby potatoes, cooked and peeled", "Small spoonfuls of roasted cumin seeds", "Red onions for salad", "G. of fresh mint", "g. de grenade", "1/4 of cup of oil", "1/2 Cup of mayonnaise", "Pinch of saffron in a little hot water", " half a lemon", "a little bit of salt", "Little black pepper" }
                        , 1, 10, "aperitivos"
                        , true, false, new string[] { "Cut the potatoes in half and place in a bowl after adding red onions and cumin seeds", "We mix the ingredients of the dressing and we pour them on the potatoes so that it takes flavor", "We continue decorating with fresh mint leaves and pomegranate seeds" });
                break;
            case 5:
                //Pasta con salmón y pesto
                //dificultad fácil-media, tiempo 40 minutos y 660 calorías por porción
                recipe = new Recipe("Pasta with salmon and pesto", 2, "portions", "22", new int[] { 1, 25, 50, 1, 1, 3, 1, 1, 175, 50, 100, 100 }, new string[] { "1/2 diente de ajo", "g. of Parmesan", "ml. of lemon juice", "Small bunch of parsley", "Bunch of basil", "olive oil spoons", "bit of salt", "bit of pepper", "g. of pasta", "g. of arugula", "g. of cherry tomatoes", "g. of smoked salmon" }
                        , 2, 10, "dieta"
                        , true, false, new string[] { "First you have to peel the garlic and cut it into large pieces", "Then we grate the parmesan", "Now we will make a puree with: Parmesan cheese, herbs, garlic and oil. We will also add lemon juice, salt and pepper", "We cook the pasta between 5 and 10 minutes", "Meanwhile we will wash the arugula, split the tomatoes in half, cut the salmon into strips, heat some oil in a frying pan and fry the tomatoes in the same along with the salmon fillets for 1 minute ", " Finally we added pesto and The arugula to the finished pasta and we mix well all the ingredients that we are missing" });
                break;
            case 6:
                //Palitos de pescado con crema de yogurt
                //dificultad fácil, 40 minutos, 550 kcal por porción
                recipe = new Recipe("Fish sticks with yoghurt cream", 2, "portions", "23", new int[] { 500, 2, 2, 1, 225, 1, 1, 1, 1, 1, 1 }, new string[] { "g. of potatoes", "Tablespoons of salad sauce", "Tablespoons of natural yogurt", "Some chives", "g. Of fish sticks (8 pieces)", "Little milk", "Spoon of margarine", "Little grated nutmeg", "Can of corn with beans", "Little of salt", "Little pepper" }
                        , 2, 30, "dieta"
                        , true, false, new string[] { "First peel the potatoes and boil them with salt for 20 minutes", "Meanwhile we will mix the salad sauce with the yogurt and we also add salt and pepper. In addition we also wash and cut the scallions and we will join them with the cream", "Mientras las patatas se siguen cociendo, descongelaremos los palitos de pescado y los freiremos en aceite", "A continuación escurrimos las patatas una vez cocidas, mezclamos además la leche y las cucharadas de margarina y se las añadimos a las patatas haciendo un puré", "Sazonamos al gusto con sal y nuez moscada", "Finally wash and drain the rest of the vegetables to put them next to the puree" });
                break;
            case 7:
                //ensalada de arroz y pollo
                //tiempo 60 minutos dificultad fácul y 600 kcal por ración
                recipe = new Recipe("Rice salad", 2, "portions", "24", new int[] { 1, 1, 1, 1, 250, 200, 1, 100, 1 }, new string[] { "Tablespoon soy sauce", "Tablespoon orange juice", "Piece of ginger", "Garlic clove", "g. Of chicken breast fillet (approx)", "g. of rice", "carrot", "g. of bean sprouts", "Little olive oil, salt and pepper" }
                        , 2, 60, "dieta"
                        , true, false, new string[] { "First we mix the soy sauce with a little salt and the orange juice", "Then peel and cut the garlic and ginger. Then put it in the pan with a little oil until a little", "Now we wash the meat, dry it and cut it into thin strips", "Now we put the rice to cook and when the water begins to evaporate we turn off the oven", "Peel and cut the carrots, rinse the shoots and also fry one minute the meat in the pan with oil and remove", "We will also fry the vegetables in that oil for one minute", "Add the rest of the ingredients (minus the rice) along with about 80 ml of water and let simmer until boiling", "Finally we salpimentamos and serve the rice" });
                break;
            case 8:
                //Verduras frescas al horno
                //dificultad fácil, 75 minutos 290 kcal por porción
                recipe = new Recipe("Baked fresh vegetables", 2, "portions", "25", new int[] { 350, 1, 1, 1, 2, 1, 3, 3 }, new string[] { "g. of potatoes", "a little bit of olive oil", "garlic clove", "a little bit of salt and pepper", "carrots", "zucchini", "Marjoram stems", "tomatoes" }
                        , 2, 75, "dieta"
                        , true, false, new string[] { "Wash the potatoes, peel them and cut them", "Grease a tray, then bake the potatoes above, chop the garlic, add it and pour a little salt and oil over", "Put the oven to preheat to 200 degrees, and then introduce the potatoes for 50 minutes", "Meanwhile: Clean and peel the vegetables and chop the zucchini and carrots. Finally about 15 minutes after we introduce the potatoes we add all the vegetables", "Finally about 2 minutes before the end of baking the potatoes and vegetables will also introduce the previously cut tomatoes and the chopped marjoram", "Add salt, pepper and you are done!" });
                break;

            case 9:
                //Batido tropical
                //muy fácil, 8 minutos
                recipe = new Recipe("Tropical milkshake", 2, "big glasses", "26", new int[] { 2, 4, 4, 4, 8 }, new string[] { "Juice cups", "Mango pieces", "Pineapple chunks", "Pieces of melon", "ice cubes" }
                        , 1, 8, "dieta"
                        , true, false, new string[] { "First we cut the pieces of mango, pineapple and melon.", "Also squeeze the oranges until getting the necessary amount and we will also chop the ice cubes", "We put the whole blender and liquid it until there is no lump (if we do not have a blender we can also use a blender). Finally we can add some sugar" });
                break;
            case 10:
                //batido rojo con avena
                //muy fácil, 2 minutos
                recipe = new Recipe("Red oatmeal", 2, "big glasses", "27", new int[] { 2, 4, 1 }, new string[] { "Cups of oat milk", "Tablespoons of red fruit mixture (can be raspberry, blackberry, strawberry, cherry ...)", "Brown sugar" }
                        , 1, 2, "dieta"
                        , true, false, new string[] { "Put all the ingredients in the blender and liquefy until the mixture is fine and ready! (If we do not have a blender we can also use a mixer)" });
                break;
            case 11:
                //batido de plátano y fresa
                //muy fácil, 2 minutos
                recipe = new Recipe("Strawberry and banana milkshake", 2, "big glasses", "28", new int[] { 8, 2, 2, 1 }, new string[] { "Strawberries", "banana", "Cups of hazelnut milk (you can use normal milk in default)", "Pinch of cinnamon" }
                        , 1, 2, "dieta"
                        , true, false, new string[] { "[Optional] Before we can put the fruit in the refrigerator or freezer so that we have a fresh and richer smoothie", "Put all the ingredients in the blender and blend until there is no lump (if we do not have a blender we can also use a mixer)" });
                break;
            case 12:
            //batido de papaya
            //muy fácil, 5 minutos
                recipe = new Recipe("Papaya milkshake", 2, "big glasses", "29", new int[] { 1, 2, 2, 2 }, new string[] { "Papaya", "Cups of oat milk", "a pinch of cinnamon", "ices" }
                    , 1, 5, "dieta"
                    , true, false, new string[] { "Pelamos la papaya y la ponemos en la batidora o licuadora junto con la leche de avena y la canela. La trituramos bien hasta que no queden grumos" });
                break;
            case 13:
                //batido de té rojo
                //muy fácil 2 minutos
                recipe = new Recipe("Red tea milkshake", 2, "portions", "27", new int[] { 2, 1, 1, 1 }, new string[] { "Cups of red tea", "Cup of strawberries or raspberry", "Spoonful of flax or chia", "To sweeten we can put some honey, sugar or brown sugar" }
                        , 1, 2, "dietas"
                        , true, false, new string[] { "Blend all or mix with a blender if we do not have a blender until we have a uniform mixture", "Pour the resulting mixture into the glasses and let them cool in the refrigerator" });
                break;
        }

    }
    //11 recipes
    public void ChooseThermomixEN (int i)
    {
        //english
        switch (i)
        {
            case 1:
                //Arroz con leche thermomix
                recipe = new Recipe("rice pudding", 6, "Portions", "30", new int[] { 1, 2, 1, 1, 1, 1, 1, 1 }, new string[] { "liter of milk", "tazas pequeñas de arroz", "small cup of rize", "a little bit of salt", "cinnamon stick", "A little bit of minced cinnamon", "lemon peels", "Anise" }
                    , 3, 45, "Thermomix"
                    , true, false, new string[] { "We add the milk, the cinnamon stick, the salt and the lemon crust and we put it to 8 minutes with speed 1 and temperature 100 degrees", "Then we add the rice and we put 30 minutes at speed 1 and 100 degrees equally", "Finally we added the sugar and the anise and we put it to more than 100 degrees with speed 1 for 5 minutes, and then it should be ready to take" });
                break;
            case 2:
                //Flan de huevo casero
                recipe = new Recipe("Pumpkin puree", 2, "Portions", "31", new int[] { 1, 1, 2, 1, 250, 1, 2, 1, 1 }, new string[] { "Zucchini ", " onion ", " carrots ", " 1/2 squash (from the long ones) ", " ml. Of water ", " salt ", " (optional) quesitos in portions ", " Little bread of the previous day ", " olive oil" }
                    , 4, 40, "Thermomix"
                    , true, false, new string[] { "First we cut the bread in thin taquitos and we will fry it in the pan without too much oil, then we leave it until they remain doraditos and we remove ", " Then we chop the onion in the thermomix 4 seconds with speed 5 and we add oil and soon to cook it to 100 degrees with speed 1 for 5 minutes ", " We add the vegetable and we program 2 minutes at speed 1 and 100 degrees ", " Then we add the water and leave it for 25 minutes, varoma, spoon speed ", " optionally we can add a pair Of quesitos and grinding at speed 6 to get the desired texture" });
                break;
            case 3:
                //Hamburgesas caseras para niños
                //tiempo de cocinado 40m , dificultad 1/5
                //350 calorías por unidad
                recipe = new Recipe("Homemade burgers for kids", 6, "Portions", "32", new int[] { 470, 50, 80, 35, 1, 1, 100, 10, 10, 0 }, new string[] { "G. Of pork fillets", "g. Of bread", "g. Of milk ", "g. Onion", " sausage (85g aproximately)", "eggs", "g. of potatoes ", "g. of Soy sauce", " g. of Mustard", "A little oil for frying" }
                    , 3, 40, "Thermomix"
                    , true, false, new string[] { "First we put the chopped bread into a bowl and we cover it with milk ", " Then we put in a glass the meat in pieces, with the salchica and the onion. And we chop them with several strokes of turbo ", " Then add the bread wet in milk, egg, cooked potatoes, mustard and soy. And we will program it at speed 6 for 10 seconds ", " Finally we shape the hamburgers and fry them with little oil in a frying pan until they are roasted" });
                break;
            case 4:
                //dulce de leche
                //tiempo de cocinado 50 minutos, dificultad 1/5
                recipe = new Recipe("Dulce de leche \n with thermomix", 4, "Portions", "33", new int[] { 380, 320, 40, 1 }, new string[] { "g. de leche condensada", "g. of evaporated milk ", " g. Of caramel liquid", "1/4 tablespoon baking soda dessert" }
                    , 3, 50, "Thermomix"
                    , true, false, new string[] { "To begin we put all the ingredients inside the thermomix and we programmed 40 minutes in speed 3 and a half with varoma temperature. To facilitate evaporation we will remove the beaker ", " Next we program 10 minutes with speed 3 and a half, but this time without temperature. To cool it down", " We finally pour it all into a large container. To be able to remove it from time to time while it cools. When we get it to room temperature, we will store it in the refrigerator until ready to serve" });
                break;
            case 5:
                //Potitos de pera, 3 horas y 14 minutos, dificultad 2 / 5 y 100 kilocalorías
                recipe = new Recipe("baby food easy to make", 2, "Portions", "31", new int[] { 400, 1 }, new string[] { "g. Pear Peeled and Peeled ", " 1/4 Orange Juice" }
                    , 5, 195, "Thermomix"
                    , false, false, new string[] { "First we will peel and remove the seeds from the pears and make sure they are clean before introducing them into the thermomix. Then we program it to 10 minutes, temperature varoma and speed 1", "Then we add the orange juice and we program To 4 minutes, temperature varoma and speed 1 ", " Finally we will grit for 1 minute with progressive speed from 7 to 9 ", " Let it cool and you are done! " });
                break;
            case 6:
                //patatas con merluza
                //4 raciones, 40 minutos dificultad 2/5, 300 kcal por ración
                recipe = new Recipe("Potatoes with hake", 4, "Portions", "34", new int[] { 600, 2, 350, 0, 300, 10, 30, 0 }, new string[] { "G. of water", "leek (We will only use the white part) ", " potatoes, we will have to peel them and cut them into large pieces ", " A little salt ", " g. Of hake without skin or spines in medium pieces", "shrimp tails or prawn tails ", " g. Of olive oil ", " parsley" }
                    , 4, 40, "Thermomix"
                    , true, false, new string[] { "First we put the leeks with water in the thermos and we program it 15 minutes to 100 degrees, turn to the left with speed spoon", "We add the salt and the potatoes and we will program it to 15 minutes, 100 degrees and turn left with speed Spoon ", " Finally we incorporate the fish, the parsley and the shrimp or prawns and the oil.We will program it to 2 minutes, varoma temperature, turn to the left and speed spoon and then let it rest for 5 minutes before serving " });
                break;
            case 7:
                //Sopa de pescado, dificultad 2/5, 50 minutos de preparación
                recipe = new Recipe("Fish Soup", 6, "Portions", "35", new int[] { 1000, 200, 100, 500, 200, 100, 1 }, new string[] { "g. of water", "g. of monkfish", "g. of shrimps", "g. mussels cleaned with warm water", "g. Of clams soaked with warm water and salt", "g. of carrots", "Chopped leek" }
                    , 4, 50, "Thermomix"
                    , true, false, new string[] { "First we will introduce in the thermomix, the bones of the monkfish, the shells and the shrimp heads (we keep the tail) Place the Varoma vessel in its position with the mussels and the clams inside.Program 18 minutes, temperature Varoma, speed 1", "We now remove the shells from the mussels and clams (and we reserve the bodies) and with the aid of a basket we put the fumet and put it in a bowl temporarily", "After washing the glass and the lid well.We put the carrots and The leek chopped and programmed at speed 5 for 4 seconds ", " Then add the oil and salt. Set 10 minutes, Varoma temperature, turn left, spoon speed ", " Next Add the choricero pepper pulp and the Set 5 minutes, Varoma temperature, turn left, spoon speed. ", "Incorporate the reserved fumet and program 10 minutes, 100º, turn left, spoon speed. When 4 minutes to finish the programmed time, add monkfish, mussels and clams.", "Finally when 1 minute to finish the programmed time, incorporate the bodies of the prawns into the nozzle, let stand for a couple of minutes, pour the soup into the soup and serve immediately." });
                break;
            case 8:
                //Peras roqueford
                //dificultad 2/5, 25 minutos
                recipe = new Recipe("Roqueford pears", 4, "Portions", "36", new int[] { 40, 2, 1, 50, 60, 0, 250 }, new string[] { "Grams of toasted hazelnuts", "pears", "juice from half a lemon", "g. of roqueford cheese vut in cubes", "g. of cream", "A pinch of: dried thyme, another pinch of freshly ground pepper and a pinch of salt", "g. de agua para el vapor" }
                    , 3, 25, "Thermomix"
                    , true, false, new string[] { "Introduce the hazelnuts in the thermomix and chop it to speed 5 for 3 seconds. We remove it in a bowl and we reserve it "," Cut the pears in half transversely and, to avoid that they oxidize, put them in a bowl with water and the juice of lemon. Make a hole in the center of each pear half by removing the pulp with the aid of a sacabola. Remove the pips and reserve the chopped pulp. "," Put roquefort cheese, cream, thyme, pepper and salt in the glass. Mix together at speed 4 for 5 seconds. "," Add the reserved pear pulp and mix 1 second with speed 4 "," Cut 4 rectangles of oven or aluminum paper and place half pear in each. Fill the holes of the pears with the mixture.", "Seal the water in the beaker, place the Varoma in position and set the Varoma temperature to 15 minutes at speed 2. Open the papillotes and serve the pears sprinkled with the hazelnut pieces"});
    
                break;
            case 9:
                //Salmonejo de espárragos
                //10 minutos, dificultad 1/5
                recipe = new Recipe("Salmorejo of asparagus", 6, "Portions", "37", new int[] { 80, 2, 2, 1, 1, 20, 50, 1 }, new string[] { "grams of olive oil", "eggs", "garlic cloves", "Jar of green asparagus drained", "Jar of white asparagus with its liquid (340 grams)", "g. of vinegar", "g. of bread", "1/2 teaspoon of salt" }
                    , 2, 10, "Thermomix"
                    , true, false, new string[] { "Put a pitcher on top of the Thermomix lid, press the balance function, weigh the oil and set aside. ", " Chop a hard egg: 2 seconds / speed 4. Remove it to a bowl and reserve it ", " Put the garlic in The thermomis and cut to 5 seconds speed 5 ", " Cut some yolks (tips) of the asparagus of the 2 colors and reserve to decorate the dish. Incorporate white asparagus with its liquid, green asparagus, vinegar, bread, salt and other hard boiled egg and set 20 seconds / speed 6. ", " Then program 2 minutes / speed 5 2 minutes / speed 3.5 y , With the beaker set, go adding the oil on the lid to fall to the glass and emulsify. Pour in an airtight container and set aside in the refrigerator. ", " Serve cold decorated with reserved asparagus yolks and accompanied with chopped hard-boiled egg." });
                break;
            case 10:
                //Solomillo de pavo con salsa de zanahoria
                //30 minutos 1/5
                recipe = new Recipe("Turkey tenderloin with carrot sauce", 5, "Portions", "38", new int[] { 3, 100, 100, 300, 40, 75, 75, 1, 0, 0 }, new string[] { "Turkey tenderloins", "g. of green pepper", "g. of red pepper", "g. of carrots", "g. of olive oil", "g. of white wine", "g. of water", "Avecrem tablet", "A little thyme", "Provencal herbs" }
                    , 2, 30, "Thermomix"
                    , true, false, new string[] { "First we wash the vegetables and we put them in the glass of the thermomix in pieces. We crush 15 seconds at speed 5. Do not worry about the size, since, later we will crush them again. ", " Now we lower the vegetables from the walls of the glass and add the oil. We programmed 10 minutes, varoma temperature, left turn, spoon speed. While we cut into the medallions the sirloins and we put them in the trays of the container Varoma. ", " Finished the time, we incorporate the wine, water, thyme, Provencal herbs and the chicken stock. We put the container Varoma in the glass and we programmed 20 minutes, varoma temperature, turn to the left, speed spoon. ", " We removed the container Varoma and we lowered with the spatula, that is in the walls of the glass. We crushed 1 minute, progressive speed 5-10. There will be a sauce a little thick, if you like more liquid, add a little water and cook again a couple of minutes at temperature varoma, speed 1." });
                break;
            case 11:
                //kingle de frambuesa
                //2 horas y 20 minutos, dificultad 3/5
                recipe = new Recipe("Raspberry kingle", 8, "Portions", "39", new int[] { 30, 4, 130, 30, 15, 1, 300 }, new string[] { "g. of sugar", "lemon peels cut in lines", "g. of milk", "g. Fresh pressed yeast", "folk", "g. of four", "a little salt" }
                    , 5, 160, "Thermomix"
                    , true, false, new string[] { "Put the sugar and lemon peel in the glass and spray 15 seconds / speed 10. Using the spatula, lower the ingredients to the bottom of the glass ", " Add milk, butter and yeast and mix 1 minutes / 37 degrees / Speed ​​2 ", " Add yolk and mix 4 seconds / speed 3 ", " Incorporate flour and salt and knead 2 min. Remove the dough from the glass and form a ball. Let it rest in a plastic bag until it doubles its volume (about 1 hour). ", " Preheat oven to 180 degrees. Line a baking sheet with baking paper and set aside. Place dough on a flour-sprinkled work surface and spread with the roll until it has a rectangle of approximately 40x50 cm. ", " Spread dough with jam, sprinkle The walnuts and roll it through the widest part. Cut the roll in half lengthwise, leaving the other end uncut. Cross the dough so that the cut is up and end the threads into a thread. ", " Put the thread in the prepared tray and bake for 20 minutes (180 ° C). Remove the kringle from the oven and let cool (30-40 minutes). Meanwhile, mix the egg white with the icing sugar in a bowl and pour over the kringle without completely covering it. Serve when cooled" });
                break;
        }
    }
    //10 recipes
    public void ChooseDrinksEN (int i)
    {
        //español
        switch (i)
        {
            //reciclado del 9 al 13 dieta sana
            case 1:
                //Batido tropical
                //muy fácil, 8 minutos
                recipe = new Recipe("Tropical milkshake", 2, "big glasses", "26", new int[] { 2, 4, 4, 4, 8 }, new string[] { "Juice cups", "Mango pieces", "Pineapple chunks", "Pieces of melon", "ice cubes" }
                        , 1, 8, "dieta"
                        , true, false, new string[] { "First we cut the pieces of mango, pineapple and melon.", "Also squeeze the oranges until getting the necessary amount and we will also chop the ice cubes", "We put the whole blender and liquid it until there is no lump (if we do not have a blender we can also use a blender). Finally we can add some sugar" });
                break;
            case 2:
                //batido rojo con avena
                //muy fácil, 2 minutos
                recipe = new Recipe("Red oatmeal", 2, "big glasses", "27", new int[] { 2, 4, 1 }, new string[] { "Cups of oat milk", "Tablespoons of red fruit mixture (can be raspberry, blackberry, strawberry, cherry ...)", "Brown sugar" }
                        , 1, 2, "dieta"
                        , true, false, new string[] { "Put all the ingredients in the blender and liquefy until the mixture is fine and ready! (If we do not have a blender we can also use a mixer)" });
                break;
            case 3:
                //batido de plátano y fresa
                //muy fácil, 2 minutos
                recipe = new Recipe("Strawberry and banana milkshake", 2, "big glasses", "28", new int[] { 8, 2, 2, 1 }, new string[] { "Strawberries", "banana", "Cups of hazelnut milk (you can use normal milk in default)", "Pinch of cinnamon" }
                        , 1, 2, "dieta"
                        , true, false, new string[] { "[Optional] Before we can put the fruit in the refrigerator or freezer so that we have a fresh and richer smoothie", "Put all the ingredients in the blender and blend until there is no lump (if we do not have a blender we can also use a mixer)" });
                break;
            case 4:
                //batido de papaya
                //muy fácil, 5 minutos
                recipe = new Recipe("Papaya milkshake", 2, "big glasses", "29", new int[] { 1, 2, 2, 2 }, new string[] { "Papaya", "Cups of oat milk", "a pinch of cinnamon", "ices" }
                        , 1, 5, "dieta"
                        , true, false, new string[] { "Pelamos la papaya y la ponemos en la batidora o licuadora junto con la leche de avena y la canela. La trituramos bien hasta que no queden grumos" });
                break;
            case 5:
                //batido de té rojo
                //muy fácil 2 minutos
                recipe = new Recipe("Red tea milkshake", 2, "portions", "27", new int[] { 2, 1, 1, 1 }, new string[] { "Cups of red tea", "Cup of strawberries or raspberry", "Spoonful of flax or chia", "To sweeten we can put some honey, sugar or brown sugar" }
                        , 1, 2, "dietas"
                        , true, false, new string[] { "Blend all or mix with a blender if we do not have a blender until we have a uniform mixture", "Pour the resulting mixture into the glasses and let them cool in the refrigerator" });
                break;
            case 6:
                //Cóctel de limón
                recipe = new Recipe("Lemon cocktail", 1, "glasses", "26", new int[] { 50, 25, 25 }, new string[] { "ml. fr ron blanco", "ml. Lima or lemon if we do not have", "ml. Of sugar syrup" }
                        , 1, 0, "bebidas"
                        , true, false, new string[] { "We put ice in the martini glass", "we put all the ingredients together with the ice in the shaker or in a closed container if we do not have", "shake the shaker and pour the liquid in the glass making a double filtrate", " We can decorate the glass with a slice of lemon or sugar on the edge " });
                break;
            case 7:
                //Chocolate mexicano de nuez y canela, 10 minutos, dificultad 2 / 5
                recipe = new Recipe("Mexican chocolate with cinnamon and nut", 4, "portions", "16", new int[] { 4, 3, 1, 1, 0, 0 }, new string[] { "cups of milk", "Cinnamon sticks", "1/2 cup peeled and sprayed nuts", "a chocolate bar (preferably Mexican)", "Cinnamon powder to decorate", "Optionally you can add whipped cream" }
                        , 1, 0, "bebidas"
                        , true, false, new string[] { "First we will mix the cinnamon, the walnuts and the chocolate tablet in a pot that will heat up to high fire. We will remove with a spoon while heating to mix the ingredients ", " We will remove until the milk begins to boil, then we will lower the temperature until a gentle boil for 5 minutes ", " Then we leave to rest 10 minutes so that the walnut is cooked And you can taste the milk ", " we can optionally paste the pieces of walnut if they bother you ", " Finally we serve it in cups putting a little cinnamon powder and cream if we wish" });
                break;
            case 8:
                //mojito casero, 10 minutos, dificultad 1/5
                recipe = new Recipe("Homemade mojito", 2, "portions", "15", new int[] { 6, 1, 0, 1, 1, 4 }, new string[] { "big spoons brown sugar", "squeezed Lemon or lime", "Peppermint or mint", "rumble", "jet of sparked water", "ices" }
                        , 1, 0, "bebidas"
                        , true, false, new string[] { "Take the number of large glasses that we need depending on the number of mojitos we want to make and squeeze the juice of half a lemon or lime in each. Then we will put three spoonfuls of brown sugar in each glass together with several mint leaves if we wish "," We maché a little the set with to mortar or manually "," We add: first the ice, second the rum and Finally we Cover the part of the free glass with gaseous water. (Ideally add 2 fingers of alcohol)", "Finally we stir well to mix everything and then we will have it ready to serve!"});
    
                break;
            case 9:
                //Sangría 10 minutos dificultad 0/5, 100 calorías
                recipe = new Recipe("Sangria", 8, "portions", "27", new int[] { 0, 0, 1, 1, 1, 1 }, new string[] { "2/3 cup lemon juice", "1/3 de taza de zumo de naranja", "1/4 cup sugar", "Lemon sliced thinly", "Sliced orange sliced thinly", "Red wine bottle" }
                        , 1, 0, "bebidas"
                        , true, false, new string[] { "In a jug mix the juices and the sugar until dissolved. Then add the lemon and orange slices to the jar to make it look nice and taste", " Add the wine to the mixture and then we can put ice or let it cool in the refrigerator if we wish, because the traditional thing is to drink it cold" });
                break;
            case 10:
                //zumo de remolacha, zanahoria y mandarina, 2 minutos 0/ 5
                recipe = new Recipe("Beet, carrot and tangerine juice", 2, "portions", "29", new int[] { 1, 1, 1, 0 }, new string[] { "Cup of peeled and sliced beet", "Cup of peeled and sliced carrots", "Cups of tangerine juice (or orange if we do not have it)", "optionally we can add ice" }
                        , 1, 0, "bebidas"
                        , true, false, new string[] { "We put all the ingredients in a blender or beat them with a blender if we do not have. And we liquefy or beat them until they are not big pieces ", " Then we glue it so that there are no stumbles ", " Finally we serve them with ice or put it in the refrigerator and let it cool" });
                break;
            case 11:
                //zumo de remolacha, zanahoria y mandarina, 2 minutos 0/ 5
                recipe = new Recipe("Fruits Smoothie", 2, "glasses", "26", new int[] { 2, 2, 2, 0 }, new string[] { "Peach", "kiwi", "banana", "soy milk" }
                        , 1, 2, "bebidas"
                        , true, false, new string[] { "First of all we will put all the ingredients in the blender", "we liquefy for 2 minutes until there is a semi liquid mass typical of smoothies" });
                break;
        }
    }
    //2 recipes
    public void ChooseMeatsEN(int i)
    {
        //español
        switch (i)
        {
            case 1:
                //Costilla de cerdo con champiñones
                //dificil, 40 minutos
                recipe = new Recipe("Pork rib with mushrooms", 4, "people", "41", new int[] { 1, 16, 75, 2, 0, 0, 0, 0 }, new string[] { "Kg of pork rib", "large mushrooms", "g of old mustard", "cloves of garlic", "a little thyme", "olive oil", "salt", "parsley" }
                        , 4, 40, "carnes"
                        , true, false, new string[] { "First we prepare the mushrooms by removing the stems and then we separate them into a bowl", "peel and chop the garlic cloves and then spread them over the mushrooms", "Then add a tablespoon of thyme, season and sprinkle with a splash Let them macerate for about 10 minutes ", " Cut the rib into pieces to put them in the fast pot, cover with water, season and add a few sprigs of parsley, add them for about 10 minutes, remove them, drain and leave them temper. ", " Heat a pan, add the mushrooms with the macerated liquid, cover and cook on both sides for about 6-8 minutes, sprinkle with chopped parsley. ", " Spread the ribs with the old mustard and Cook them on the barbecue for 3-4 minutes on each side, then serve them on the plate and that's it! " });
                break;
            case 2:
                //Alitas de pollo al horno
                //fácil, 40 minutos
                recipe = new Recipe("Baked chicken wings", 2, "people", "42", new int[] { 10, 1, 3, 1, 2, 0 }, new string[] { "chicken wings", "spoonful of honey", "spoonfuls of soy sauce", "teaspoon of paprika", "spoonfuls of olive oil", "salt" }
                        , 2, 40, "carnes"
                        , true, false, new string[] { "Primero cortaremos las alitas en 3, siguiendo la articulación y elimina la punta que carece de carne. Con un cuchillo retírales las partes sobrantes de piel (lugar en el que se concentra toda la grasa) y extiéndelas sobre la placa de horno.", "Pon en un cuenco la miel, la salsa de soja, el pimentón y las cucharadas de aceite. Bate todo bien con un tenedor hasta que quede bien unificado.", "Unta las alitas con la mezcla anterior, sálalas e introduce la bandeja en el horno (previamente calentado durante 10 minutos) a 190 grados durante 25 minutos. Retíralas y sirve." });
                break;
            case 3:
                //Raxo con patatas y pimientos de padrón
                //media, 30 minutos
                recipe = new Recipe("Raxo with potatoes and peppers", 4, "people", "43", new int[] { 1, 3, 0, 0, 0, 500, 250 }, new string[] { "kg of pork loin", "garlic cloves", "fine salt", "olive oil", "black pepper", "g of potatoes", "g of padrón peppers" }
                        , 3, 30, "carnes"
                        , true, false, new string[] { "raxo means pork loin in pieces, so the best is to cut the meat right when we go to cook the recipe.If you go to the butcher the same day, you can ask them to cut the meat so you save work.", " We remove the fat from the meat if we want to. Thenm cut it into slices of 3 cm and then these into cubes (3 cm x 3 cm. We make this process with the whole piece. ", " Chop the finite garlic in a bowl, toss the meat and marinate with the garlic, salt, black pepper (freshly ground if you can be) and a jet of oil. In Galicia sites you will also find the raxo with a good jet of white wine and a touch of oregano.Although in today's recipe we have opted for the most general recipe. ", " We remove everything very well with a wooden spoon, to integrate well the ingredients and that the meat catch all the flavor. ", " We cover everything with a rag or the lid of the bowl so that the refrigerator does not I caught smells. We leave it resting in the fridge for a whole day. ", " When there are 10 minutes to complete the time, we will prepare the potatoes. We peel them and cut them in the traditional style, throughout. We put them to fry in olive oil. " });

                break;
        }
    }
}
