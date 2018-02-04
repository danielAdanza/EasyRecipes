using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class IndexScript : MonoBehaviour {

    public Text textDessert;
    public Text textAppetizer;
    public Text textDiet;
    public Text textThermomix;
    public Text textDrinks;
    public Text textOwn;
    public Text textMeat;
    public Text textIndex;
    //it will contain an array of all the titles list for this category and languaje
    string[] titles;
    public GameObject MainIndex;
    //list of recipes (sub-index)
    public GameObject recipesObject;
    public GameObject recipesList;

    public AudioSource audio;

    public GameObject buttonBackToIndex;
    public GameObject buttonBackToCover;

    //in the beginning of everything we will check for the languaje
    void Start()
    {

        buttonBackToIndex.SetActive(false);
        buttonBackToCover.SetActive(true);

        //Languaje Recipe:
        //LAN 1: spanish
        //LAN 2: english

        if (UnityEngine.PlayerPrefs.GetInt("sound") != 1)
        {
            audio.Play();
        }

        if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
        {
            textDessert.text = "1) Postres";
            textAppetizer.text = "2) Aperitivos";
            textDiet.text = "3) Dieta";
            textThermomix.text = "4) Termomix";
            textDrinks.text = "5) Bebidas";
            textOwn.text = "6) Recetas propias";
            textMeat.text = "6) Carnes";
            textIndex.text = "Índice";
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (buttonBackToCover.activeInHierarchy == true )
            {
                GoBackToCover();
            }
            else
            {
                GoBackToIndex();
            }
        }
    }

    //it saves the category of the recipe and goes to the next step
    public void OnCategoryClicked (int category)
    {
        buttonBackToIndex.SetActive(true);
        buttonBackToCover.SetActive(false);

        if (UnityEngine.PlayerPrefs.GetInt("sound") != 1)
        {
            audio.Play();
        }

        UnityEngine.PlayerPrefs.SetInt("category",category);

        switch (category)
        {
            case 1:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseDessertSP(); }
                else
                { ChooseDessertEN(); }
                break;
            case 2:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseAppetizerSP(); }
                else
                { ChooseAppetizerEN(); }
                break;
            case 3:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseDietSP(); }
                else
                { ChooseDietEN(); }
                break;
            case 4:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseThermomixSP(); }
                else
                { ChooseThermomixEN(); }
                break;
            case 5:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseDrinksSP(); }
                else
                { ChooseDrinksEN(); }
                break;
            case 6:
                if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
                { ChooseMeatSP(); }
                else
                { ChooseMeatEN(); }
                break;
        }

        MainIndex.SetActive(false);
        recipesList.SetActive(true);

        //creating the ingredient list
        for (int i = recipesList.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(recipesList.transform.GetChild(i).gameObject);
        }

        //it goes for each different recipe that we have in the category
        for (int i = 0; i < titles.Length; i++)
        {
            //we create another variable with scope inside this loop so we don't loose the value
            var j = i;
            //it adds the recipeObject to the list and it sets it like a child object
            GameObject recipesPanel;
            recipesPanel = Instantiate(recipesObject) as GameObject;
            recipesPanel.transform.SetParent(recipesList.transform);

            //it finds the label object and it changes its value
            recipesPanel.transform.Find("Label").GetComponent<Text>().text = UnityEngine.PlayerPrefs.GetInt("category") + "." + (i+1) + ") " + titles[i];

            //it creates the event click for each button
            Button myButton = recipesPanel.transform.Find("Button").GetComponent<Button>();
            myButton.GetComponent<Button>().onClick.AddListener(() => { OnRecipeClicked(j); });
        }
    }
    
    //when a recipe gets clicked it will save the id number of the recipe
    //and it will call the next scene for recipes
    public void OnRecipeClicked (int recipeNumber)
    {
        UnityEngine.PlayerPrefs.SetInt("recipeNumber", recipeNumber + 1);
        SceneManager.LoadScene("recipeDescription");
    }

    public void GoBackToCover ()
    {
        SceneManager.LoadScene("mainMenu");
    }

    public void GoBackToIndex()
    {

        buttonBackToIndex.SetActive(false);
        buttonBackToCover.SetActive(true);

        MainIndex.SetActive(true);
        recipesList.SetActive(false);

        if (UnityEngine.PlayerPrefs.GetInt("languaje") == 1)
        {
            textIndex.text = "Índice";
        }
        else
        {
            textIndex.text = "Index";
        }
    }

    //SPANISH RECIPES
    //12 recipes
    public void ChooseDessertSP()
    {
        textIndex.text = "Postres (12)";
        titles = new string[] { "Tarta de queso y chocolate", "Cupcakes de chocolate", "Tarta de huesitos", "Tortitas caseras",
                                "Volcán de chololate", "Mousse de limón", "Mousse de chocolate", "Bizcocho casero", "Galletas de almendra",
                                "Gominolas caseras", "Leche frita", "Flan"};

        return;
    }
    //11 recipes
    public void ChooseAppetizerSP()
    {
        textIndex.text = "Aperitivo (12)";
        titles = new string[] { "Almejas al ajillo", "Gofres caseros", "pastél Japonés", "Canapés Tropicales", "patatas asadas al microondas",
                                "Ensalada de tomate", "Ensalada carpese", "Ensalada de tomate y maíz", "Ensalada de patata",
                                "Pincho de salmón", "Pincho de gambas empanadas", "Pincho jamón, queso y tomate cherry"};

        return;

    }
    //13 recipes
    public void ChooseDietSP()
    {
        textIndex.text = "Dieta (13)";
        titles = new string[] { "Ensalada de tomate", "Ensalada carpese", "Ensalada de tomate y maíz", "Ensalada de patata",
                                "Pasta con salmón y pesto", "palitos de pescado con crema de yogurt", "Ensalada de arroz", "Verduras frescas al horno",
                                "Batido tropical", "Batido rojo avena", "Batido de fresa y plátano", "Batido de papaya", "Batido de té rojo" };

        return;

    }
    //11 recipes
    public void ChooseThermomixSP()
    {
        textIndex.text = "Thermomix (11)";
        titles = new string[] { "Arroz con leche", "Puré de calabaza", "Hamburguesas caseras para niños", "Dulce de leche casero con thermomix",
                                "Potitos facilísimos de pera", "Patatas con merluza", "Sopa de pescado", "Peras roqueford", "Salmonejo de espárragos",
                                "Solomillo de pavo con salsa de zanahoria", "Kingle de frambuesa" };

        return;
    }
    //10 recipes
    public void ChooseDrinksSP()
    {
        textIndex.text = "Bebidas (11)";
        titles = new string[] { "Batido tropical", "Batido rojo avena", "Batido de fresa y plátano", "Batido de papaya",
                                "Batido de té rojo", "Cóctel de limón", "Chocolate mexicano de nuez y canela", "Mojito casero",
                                "Sangría", "zumo de remolacha, zanahoria y mandarina", "Smoothie de Frutas"};

        return;
    }
    //3 recipes
    public void ChooseMeatSP()
    {
        textIndex.text = "Carnes (3)";

        titles = new string[] { "Costilla de Cerdo con champiñones", "Alitas de pollo asadas al horno", "Raxo con patatas y pimientos de padrón" };

        return;
    }

    //ENGLISH RECIPES
    //12 recipes
    public void ChooseDessertEN()
    {
        textIndex.text = "Desserts (12)";

        titles = new string[] { "Chocolate Cheesecake", "Chocolate Cupcakes", "Huesitos cake", "Homemade Pancakes", "Chololate volcano",
                                "Lemon Mousse", "Chocolate Mousse", "Homemade biscuit", "Almond cookies", "Homemade candies",
                                "Fried milk", "Flan (Spanish Pudding)"};

        return;

    }
    //11 recipes
    public void ChooseAppetizerEN()
    {
        textIndex.text = "Appetizer (12)";

        titles = new string[] { "Clams with garlic", "Homemade Wafles", "Japanese cake", "tropical canapes", "Microwave roasted potatoes", "Tomato salad",
                                "Carpese Salad", "Tomato and corn salad", "Potato salad", "Pincho of salmon", "Pincho of breaded shrimps", "Pincho of Ham, cheese and cherry tomato" };

        return;

    }
    //13 recipes
    public void ChooseDietEN()
    {
        textIndex.text = "Diet (13)";

        titles = new string[] { "Tomato salad", "Carpese Salad", "Tomato and corn salad", "Potato salad", "Pasta with salmon and pesto",
                                "Fish sticks with yoghurt cream", "Rice salad", "Baked fresh vegetables", "Tropical milkshake", "Red oatmeal",
                                "Strawberry and banana milkshake", "Papaya milkshake", "Red tea milkshake" };
        return;

    }
    //11 recipes
    public void ChooseThermomixEN()
    {
        textIndex.text = "Thermomix (11)";

        titles = new string[] { "rice pudding", "Pumpkin puree", "Homemade burgers for kids", "homemade Dulce de leche with thermomix",
                                "baby food easy to make", "Potatoes with hake", "Sopa de pescado", "Roqueford pears", "Salmorejo of asparagus",
                                "Salmorejo of asparagus", "Turkey tenderloin with carrot sauce", "Raspberry kingle" };

        return;

    }
    //10 recipes
    public void ChooseDrinksEN()
    {
        textIndex.text = "Drinks (11)";

        titles = new string[] { "Tropical milkshake", "Red oatmeal", "Strawberry and banana milkshake", "Papaya milkshake",
                                "Red tea milkshake", "Lemon cocktail", "Mexican chocolate with cinnamon and nut", "Homemade mojito",
                                "Sangria", "Beet, carrot and tangerine juice", "Fruits Smoothie" };

        return;
    }
    //3 recipes
    public void ChooseMeatEN()
    {
        textIndex.text = "Meats (3)";

        titles = new string[] { "Pork rib with mushrooms", "Baked chicken wings", "Raxo with potatoes and peppers" };

        return;
    }

}
