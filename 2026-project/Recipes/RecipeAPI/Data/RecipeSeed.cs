using Microsoft.EntityFrameworkCore;
using RecipeAPI.Entities;

namespace RecipeAPI.Data;

internal static class RecipeSeed
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        string[] categoryNames =
        [
            "Breakfast", "Soups", "Salads", "Pasta", "Rice",
            "Chicken", "Beef", "Fish", "Vegetarian", "Desserts"
        ];
        string[] ingredientNames =
        [
            "Flour", "Eggs", "Milk", "Butter", "Olive oil", "Salt", "Black pepper",
            "Sugar", "Tomato", "Onion", "Garlic", "Carrot", "Potato", "Rice",
            "Pasta", "Chicken breast", "Ground beef", "Salmon", "Canned tuna",
            "Cheese", "Yogurt", "Spinach", "Broccoli", "Mushroom", "Bell pepper",
            "Zucchini", "Lemon", "Oats", "Lentils", "Banana"
        ];

        modelBuilder.Entity<Category>().HasData(categoryNames.Select((name, index) =>
            new Category { Id = index + 1, Name = name }));
        modelBuilder.Entity<Ingredient>().HasData(ingredientNames.Select((name, index) =>
            new Ingredient { Id = index + 1, Name = name }));

        var recipes = new List<Recipe>();
        var recipeIngredients = new List<RecipeIngredient>();

        Add(1, 1, "Classic Pancakes", 20, 4, "Whisk flour, eggs and milk. Melt butter in a pan and cook small pancakes until golden on both sides.",
            (1, 200m, "g"), (2, 2m, "whole"), (3, 300m, "ml"), (4, 20m, "g"));
        Add(2, 1, "Banana Oat Pancakes", 20, 2, "Mash banana, mix with oats, eggs and milk. Cook spoonfuls in a buttered pan until set on both sides.",
            (30, 1m, "whole"), (28, 100m, "g"), (2, 2m, "whole"), (3, 50m, "ml"), (4, 10m, "g"));
        Add(3, 1, "Spinach Omelet", 15, 2, "Wilt spinach in oil. Beat eggs with salt, pour into the pan and cook until set. Fold over the cheese.",
            (22, 100m, "g"), (5, 1m, "tbsp"), (2, 4m, "whole"), (6, 0.25m, "tsp"), (20, 50m, "g"));
        Add(4, 1, "Mushroom Scrambled Eggs", 15, 2, "Saute sliced mushrooms in butter. Add beaten eggs and stir over low heat until cooked. Season with pepper.",
            (24, 150m, "g"), (4, 15m, "g"), (2, 4m, "whole"), (7, 0.25m, "tsp"));
        Add(5, 1, "Banana Porridge", 10, 2, "Simmer oats in milk until soft, stirring frequently. Top with sliced banana.",
            (28, 100m, "g"), (3, 400m, "ml"), (30, 1m, "whole"));
        Add(6, 1, "Yogurt Oat Bowl", 5, 2, "Divide yogurt between bowls. Stir in oats and top with sliced banana.",
            (21, 300m, "g"), (28, 60m, "g"), (30, 1m, "whole"));
        Add(7, 1, "Potato Breakfast Hash", 30, 2, "Dice potatoes and onion. Fry in oil until potatoes are tender. Add beaten eggs and cook through.",
            (13, 300m, "g"), (10, 1m, "whole"), (5, 2m, "tbsp"), (2, 2m, "whole"));
        Add(8, 1, "Tomato Cheese Omelet", 15, 2, "Soften chopped tomato in butter. Add beaten eggs, cook until set and fold over grated cheese.",
            (9, 2m, "whole"), (4, 10m, "g"), (2, 4m, "whole"), (20, 60m, "g"));
        Add(9, 1, "Pepper Egg Skillet", 25, 2, "Saute sliced pepper and onion in oil. Add chopped tomato and simmer. Make wells, add eggs and cover until whites are set.",
            (25, 1m, "whole"), (10, 1m, "whole"), (5, 1m, "tbsp"), (9, 3m, "whole"), (2, 4m, "whole"));
        Add(10, 1, "Savory Oatmeal with Egg", 15, 2, "Simmer oats in 400 ml water with salt. Wilt spinach in butter, fry eggs and serve over the oats.",
            (28, 100m, "g"), (6, 0.25m, "tsp"), (22, 80m, "g"), (4, 10m, "g"), (2, 2m, "whole"));

        Add(11, 2, "Tomato Soup", 35, 4, "Saute onion and garlic in oil. Add chopped tomatoes and 600 ml water. Simmer for 25 minutes, blend and season.",
            (10, 1m, "whole"), (11, 2m, "cloves"), (5, 1m, "tbsp"), (9, 800m, "g"), (6, 0.5m, "tsp"));
        Add(12, 2, "Carrot Soup", 35, 4, "Saute onion in butter. Add sliced carrots and 800 ml water. Simmer until tender, blend and season.",
            (10, 1m, "whole"), (4, 20m, "g"), (12, 600m, "g"), (6, 0.5m, "tsp"));
        Add(13, 2, "Potato Leek-Free Soup", 35, 4, "Soften onion in butter. Add diced potatoes and 700 ml water. Simmer until tender, stir in milk and blend.",
            (10, 2m, "whole"), (4, 20m, "g"), (13, 600m, "g"), (3, 200m, "ml"));
        Add(14, 2, "Broccoli Cheese Soup", 30, 4, "Simmer broccoli and diced potato in 700 ml salted water until tender. Blend, add milk and stir in cheese over low heat.",
            (23, 400m, "g"), (13, 200m, "g"), (6, 0.5m, "tsp"), (3, 200m, "ml"), (20, 100m, "g"));
        Add(15, 2, "Mushroom Soup", 30, 4, "Saute mushrooms and onion in butter. Stir in flour, then milk and 500 ml water. Simmer for 15 minutes and season.",
            (24, 400m, "g"), (10, 1m, "whole"), (4, 30m, "g"), (1, 20m, "g"), (3, 300m, "ml"), (6, 0.5m, "tsp"));
        Add(16, 2, "Lentil Tomato Soup", 45, 4, "Saute onion and carrot in oil. Add rinsed lentils, chopped tomato and 1 litre water. Simmer until lentils are tender.",
            (10, 1m, "whole"), (12, 2m, "whole"), (5, 1m, "tbsp"), (29, 200m, "g"), (9, 3m, "whole"));
        Add(17, 2, "Chicken Rice Soup", 40, 4, "Simmer diced chicken, carrot, onion and rice in 1.2 litres salted water until rice is tender and chicken reaches 74 C.",
            (16, 300m, "g"), (12, 2m, "whole"), (10, 1m, "whole"), (14, 100m, "g"), (6, 0.5m, "tsp"));
        Add(18, 2, "Spinach Potato Soup", 35, 4, "Soften garlic in oil. Add diced potato and 800 ml water. Simmer until tender, add spinach, cook for 3 minutes and blend.",
            (11, 2m, "cloves"), (5, 1m, "tbsp"), (13, 400m, "g"), (22, 250m, "g"));
        Add(19, 2, "Vegetable Pasta Soup", 35, 4, "Saute onion in oil. Add diced carrot, zucchini, tomato and 1 litre water. Simmer for 15 minutes, add pasta and cook until tender.",
            (10, 1m, "whole"), (5, 1m, "tbsp"), (12, 2m, "whole"), (26, 1m, "whole"), (9, 3m, "whole"), (15, 100m, "g"));
        Add(20, 2, "Beef Potato Soup", 45, 4, "Brown beef and onion in oil. Add diced potatoes, carrots and 1 litre water. Simmer until vegetables are tender and beef reaches 71 C.",
            (17, 300m, "g"), (10, 1m, "whole"), (5, 1m, "tbsp"), (13, 300m, "g"), (12, 2m, "whole"));

        Add(21, 3, "Tomato Cheese Salad", 10, 2, "Slice tomatoes and cheese. Arrange on a plate and drizzle with olive oil and lemon juice.",
            (9, 3m, "whole"), (20, 150m, "g"), (5, 1m, "tbsp"), (27, 0.5m, "whole"));
        Add(22, 3, "Spinach Egg Salad", 20, 2, "Hard-boil eggs for 10 minutes, cool and slice. Toss washed spinach with oil and lemon juice, then add eggs.",
            (2, 3m, "whole"), (22, 150m, "g"), (5, 1m, "tbsp"), (27, 0.5m, "whole"));
        Add(23, 3, "Tuna Tomato Salad", 10, 2, "Drain tuna. Toss with chopped tomato, finely sliced onion, olive oil and lemon juice.",
            (19, 200m, "g"), (9, 3m, "whole"), (10, 0.5m, "whole"), (5, 1m, "tbsp"), (27, 0.5m, "whole"));
        Add(24, 3, "Lentil Carrot Salad", 35, 4, "Boil lentils in water until tender, drain and cool. Mix with grated carrot, diced pepper, olive oil and lemon juice.",
            (29, 200m, "g"), (12, 2m, "whole"), (25, 1m, "whole"), (5, 2m, "tbsp"), (27, 1m, "whole"));
        Add(25, 3, "Potato Yogurt Salad", 30, 4, "Boil diced potatoes until tender and cool. Toss with yogurt, finely chopped onion and salt.",
            (13, 600m, "g"), (21, 150m, "g"), (10, 0.5m, "whole"), (6, 0.5m, "tsp"));
        Add(26, 3, "Broccoli Cheese Salad", 15, 2, "Blanch broccoli for 3 minutes and cool. Toss with diced cheese and a yogurt and lemon dressing.",
            (23, 300m, "g"), (20, 80m, "g"), (21, 100m, "g"), (27, 0.5m, "whole"));
        Add(27, 3, "Roasted Vegetable Salad", 40, 4, "Dice zucchini, pepper and carrot. Toss with oil and roast at 200 C for 30 minutes. Cool slightly and add lemon juice.",
            (26, 2m, "whole"), (25, 2m, "whole"), (12, 2m, "whole"), (5, 2m, "tbsp"), (27, 1m, "whole"));
        Add(28, 3, "Chicken Spinach Salad", 25, 2, "Pan-fry chicken in oil until its centre reaches 74 C. Slice and serve over washed spinach and tomato with lemon juice.",
            (16, 250m, "g"), (5, 1m, "tbsp"), (22, 150m, "g"), (9, 2m, "whole"), (27, 0.5m, "whole"));
        Add(29, 3, "Rice Pepper Salad", 30, 4, "Cook rice in water, drain and cool promptly. Toss with diced pepper, tomato, olive oil and lemon juice. Refrigerate until serving.",
            (14, 200m, "g"), (25, 2m, "whole"), (9, 2m, "whole"), (5, 2m, "tbsp"), (27, 1m, "whole"));
        Add(30, 3, "Warm Mushroom Spinach Salad", 15, 2, "Saute mushrooms and garlic in oil. Toss with washed spinach just until wilted and finish with lemon juice.",
            (24, 250m, "g"), (11, 1m, "cloves"), (5, 1m, "tbsp"), (22, 150m, "g"), (27, 0.5m, "whole"));

        Add(31, 4, "Garlic Olive Oil Pasta", 20, 2, "Boil pasta in salted water. Gently cook sliced garlic in oil and toss with drained pasta and a splash of cooking water.",
            (15, 200m, "g"), (6, 1m, "tsp"), (11, 3m, "cloves"), (5, 2m, "tbsp"));
        Add(32, 4, "Fresh Tomato Pasta", 25, 2, "Boil pasta. Saute garlic in oil, add chopped tomatoes and simmer for 15 minutes. Toss with drained pasta.",
            (15, 200m, "g"), (11, 2m, "cloves"), (5, 1m, "tbsp"), (9, 4m, "whole"));
        Add(33, 4, "Mushroom Milk Pasta", 25, 2, "Boil pasta. Saute mushrooms in butter, stir in flour and milk, simmer until thickened and toss with pasta.",
            (15, 200m, "g"), (24, 250m, "g"), (4, 20m, "g"), (1, 10m, "g"), (3, 200m, "ml"));
        Add(34, 4, "Spinach Cheese Pasta", 20, 2, "Boil pasta. Wilt spinach in butter, add milk and cheese over low heat, then toss with drained pasta.",
            (15, 200m, "g"), (22, 200m, "g"), (4, 15m, "g"), (3, 100m, "ml"), (20, 80m, "g"));
        Add(35, 4, "Tuna Tomato Pasta", 25, 2, "Boil pasta. Soften onion in oil, add chopped tomato and simmer. Stir in drained tuna and pasta and heat through.",
            (15, 200m, "g"), (10, 1m, "whole"), (5, 1m, "tbsp"), (9, 3m, "whole"), (19, 200m, "g"));
        Add(36, 4, "Chicken Broccoli Pasta", 30, 4, "Boil pasta, adding broccoli for the final 4 minutes. Fry diced chicken in oil to 74 C, add milk and cheese, then toss everything together.",
            (15, 350m, "g"), (23, 300m, "g"), (16, 400m, "g"), (5, 1m, "tbsp"), (3, 150m, "ml"), (20, 100m, "g"));
        Add(37, 4, "Beef Tomato Pasta", 40, 4, "Brown beef with onion and garlic in oil. Add chopped tomatoes and simmer for 25 minutes, ensuring beef reaches 71 C. Serve with boiled pasta.",
            (17, 400m, "g"), (10, 1m, "whole"), (11, 2m, "cloves"), (5, 1m, "tbsp"), (9, 6m, "whole"), (15, 350m, "g"));
        Add(38, 4, "Zucchini Lemon Pasta", 25, 2, "Boil pasta. Saute sliced zucchini and garlic in oil, then toss with pasta, lemon juice and grated cheese.",
            (15, 200m, "g"), (26, 2m, "whole"), (11, 1m, "cloves"), (5, 1m, "tbsp"), (27, 0.5m, "whole"), (20, 40m, "g"));
        Add(39, 4, "Baked Cheese Pasta", 40, 4, "Boil pasta. Melt butter, stir in flour and gradually add milk. Add cheese, combine with pasta and bake at 190 C for 20 minutes.",
            (15, 350m, "g"), (4, 30m, "g"), (1, 30m, "g"), (3, 500m, "ml"), (20, 200m, "g"));
        Add(40, 4, "Lentil Pasta Sauce", 45, 4, "Saute onion and carrot in oil. Add lentils, chopped tomatoes and 500 ml water. Simmer until lentils are tender and serve over boiled pasta.",
            (10, 1m, "whole"), (12, 1m, "whole"), (5, 1m, "tbsp"), (29, 150m, "g"), (9, 5m, "whole"), (15, 300m, "g"));

        Add(41, 5, "Vegetable Fried Rice", 25, 2, "Cook rice in water and drain. Stir-fry diced carrot and pepper in oil, add rice and beaten eggs, and stir until eggs are cooked.",
            (14, 150m, "g"), (12, 1m, "whole"), (25, 1m, "whole"), (5, 1m, "tbsp"), (2, 2m, "whole"));
        Add(42, 5, "Mushroom Rice", 30, 4, "Saute onion and mushrooms in butter. Add rice and 600 ml water. Cover and simmer until rice is tender, adding water as needed.",
            (10, 1m, "whole"), (24, 300m, "g"), (4, 20m, "g"), (14, 300m, "g"));
        Add(43, 5, "Tomato Rice", 30, 4, "Saute garlic in oil. Add rice, chopped tomatoes and 500 ml water. Cover and simmer until rice is tender, adding water as needed.",
            (11, 2m, "cloves"), (5, 1m, "tbsp"), (14, 300m, "g"), (9, 4m, "whole"));
        Add(44, 5, "Lemon Spinach Rice", 25, 4, "Cook rice in salted water. Wilt spinach in oil and fold into the rice with lemon juice.",
            (14, 300m, "g"), (6, 0.5m, "tsp"), (22, 250m, "g"), (5, 1m, "tbsp"), (27, 1m, "whole"));
        Add(45, 5, "Chicken Rice Skillet", 40, 4, "Brown diced chicken and onion in oil. Add rice, diced pepper and 650 ml water. Cover and simmer until rice is tender and chicken reaches 74 C.",
            (16, 500m, "g"), (10, 1m, "whole"), (5, 1m, "tbsp"), (14, 300m, "g"), (25, 1m, "whole"));
        Add(46, 5, "Beef Pepper Rice", 35, 4, "Brown beef and onion in oil. Add diced pepper, rice and 600 ml water. Simmer covered until rice is tender and beef reaches 71 C.",
            (17, 400m, "g"), (10, 1m, "whole"), (5, 1m, "tbsp"), (25, 2m, "whole"), (14, 300m, "g"));
        Add(47, 5, "Lentil Rice Pilaf", 45, 4, "Boil lentils until nearly tender and drain. Saute onion in oil, add rice, lentils and 600 ml water. Simmer covered until tender.",
            (29, 150m, "g"), (10, 2m, "whole"), (5, 2m, "tbsp"), (14, 250m, "g"));
        Add(48, 5, "Broccoli Cheese Rice", 30, 4, "Cook rice in water, adding broccoli during the final 5 minutes. Drain excess water and stir in butter and grated cheese.",
            (14, 300m, "g"), (23, 350m, "g"), (4, 20m, "g"), (20, 120m, "g"));
        Add(49, 5, "Tuna Rice Bowl", 25, 2, "Cook rice in water and drain. Top with drained tuna, diced tomato and a yogurt and lemon dressing.",
            (14, 150m, "g"), (19, 200m, "g"), (9, 2m, "whole"), (21, 100m, "g"), (27, 0.5m, "whole"));
        Add(50, 5, "Zucchini Carrot Rice", 30, 4, "Saute grated carrot and diced zucchini in oil. Add rice and 600 ml salted water. Cover and simmer until tender.",
            (12, 2m, "whole"), (26, 2m, "whole"), (5, 1m, "tbsp"), (14, 300m, "g"), (6, 0.5m, "tsp"));

        Add(51, 6, "Lemon Garlic Chicken", 30, 4, "Coat chicken with oil, crushed garlic, lemon juice and salt. Bake at 200 C until the thickest part reaches 74 C.",
            (16, 600m, "g"), (5, 2m, "tbsp"), (11, 3m, "cloves"), (27, 1m, "whole"), (6, 0.5m, "tsp"));
        Add(52, 6, "Chicken Potato Tray Bake", 45, 4, "Dice potatoes and carrots, toss with chicken and oil, and roast at 200 C until vegetables are tender and chicken reaches 74 C.",
            (13, 600m, "g"), (12, 3m, "whole"), (16, 600m, "g"), (5, 2m, "tbsp"));
        Add(53, 6, "Chicken Mushroom Skillet", 30, 4, "Brown sliced chicken in butter. Add mushrooms and garlic, then milk. Simmer until chicken reaches 74 C and sauce reduces.",
            (16, 600m, "g"), (4, 25m, "g"), (24, 300m, "g"), (11, 2m, "cloves"), (3, 200m, "ml"));
        Add(54, 6, "Chicken Pepper Stir-Fry", 25, 4, "Slice chicken and peppers. Stir-fry chicken in oil, add peppers and garlic, and cook until chicken reaches 74 C.",
            (16, 600m, "g"), (25, 3m, "whole"), (5, 2m, "tbsp"), (11, 2m, "cloves"));
        Add(55, 6, "Yogurt Baked Chicken", 40, 4, "Coat chicken in yogurt, lemon juice, crushed garlic and salt. Bake at 200 C until the centre reaches 74 C.",
            (16, 600m, "g"), (21, 150m, "g"), (27, 1m, "whole"), (11, 2m, "cloves"), (6, 0.5m, "tsp"));
        Add(56, 6, "Chicken Tomato Stew", 40, 4, "Brown diced chicken and onion in oil. Add chopped tomatoes and 200 ml water. Simmer until chicken reaches 74 C and sauce thickens.",
            (16, 600m, "g"), (10, 1m, "whole"), (5, 1m, "tbsp"), (9, 6m, "whole"));
        Add(57, 6, "Chicken Spinach Bake", 35, 4, "Place chicken in an oiled baking dish with spinach and milk. Top with cheese and bake at 190 C until chicken reaches 74 C.",
            (16, 600m, "g"), (5, 1m, "tbsp"), (22, 250m, "g"), (3, 150m, "ml"), (20, 100m, "g"));
        Add(58, 6, "Chicken Broccoli Skillet", 25, 4, "Fry diced chicken in oil. Add broccoli, garlic and 100 ml water. Cover and cook until broccoli is tender and chicken reaches 74 C.",
            (16, 600m, "g"), (5, 1m, "tbsp"), (23, 400m, "g"), (11, 2m, "cloves"));
        Add(59, 6, "Chicken Zucchini Boats", 45, 4, "Halve zucchini and scoop out centres. Cook diced chicken with tomato and oil to 74 C. Fill zucchini, top with cheese and bake at 190 C for 25 minutes.",
            (26, 4m, "whole"), (16, 400m, "g"), (9, 3m, "whole"), (5, 1m, "tbsp"), (20, 100m, "g"));
        Add(60, 6, "Chicken Lentil Stew", 50, 4, "Saute onion and carrot in oil. Add diced chicken, lentils and 900 ml water. Simmer until lentils are tender and chicken reaches 74 C.",
            (10, 1m, "whole"), (12, 2m, "whole"), (5, 1m, "tbsp"), (16, 400m, "g"), (29, 200m, "g"));

        Add(61, 7, "Simple Beef Patties", 25, 4, "Mix beef with finely chopped onion, egg and salt. Shape four patties and pan-fry in oil until the centre reaches 71 C.",
            (17, 500m, "g"), (10, 1m, "whole"), (2, 1m, "whole"), (6, 0.5m, "tsp"), (5, 1m, "tbsp"));
        Add(62, 7, "Beef Stuffed Peppers", 55, 4, "Cook rice. Brown beef and onion in oil to 71 C, stir in rice and chopped tomato. Fill halved peppers and bake at 190 C for 30 minutes.",
            (14, 150m, "g"), (17, 400m, "g"), (10, 1m, "whole"), (5, 1m, "tbsp"), (9, 3m, "whole"), (25, 4m, "whole"));
        Add(63, 7, "Beef Potato Bake", 50, 4, "Boil sliced potatoes until nearly tender. Brown beef and onion to 71 C, layer with potatoes and milk, top with cheese and bake at 190 C for 25 minutes.",
            (13, 700m, "g"), (17, 500m, "g"), (10, 1m, "whole"), (3, 150m, "ml"), (20, 100m, "g"));
        Add(64, 7, "Beef Mushroom Skillet", 25, 4, "Brown beef and onion in butter. Add sliced mushrooms and garlic, and cook until mushrooms soften and beef reaches 71 C.",
            (17, 500m, "g"), (10, 1m, "whole"), (4, 20m, "g"), (24, 300m, "g"), (11, 2m, "cloves"));
        Add(65, 7, "Beef Tomato Meatballs", 40, 4, "Mix beef with egg and oats, shape small balls and brown in oil. Add chopped tomatoes and simmer until meatballs reach 71 C.",
            (17, 500m, "g"), (2, 1m, "whole"), (28, 40m, "g"), (5, 1m, "tbsp"), (9, 6m, "whole"));
        Add(66, 7, "Beef Zucchini Skillet", 25, 4, "Brown beef in oil with garlic. Add diced zucchini and tomato, then simmer until vegetables soften and beef reaches 71 C.",
            (17, 500m, "g"), (5, 1m, "tbsp"), (11, 2m, "cloves"), (26, 3m, "whole"), (9, 3m, "whole"));
        Add(67, 7, "Beef Lentil Pot", 50, 4, "Brown beef and onion in oil. Add lentils, diced carrot and 800 ml water. Simmer until lentils are tender and beef reaches 71 C.",
            (17, 400m, "g"), (10, 1m, "whole"), (5, 1m, "tbsp"), (29, 200m, "g"), (12, 2m, "whole"));
        Add(68, 7, "Beef Broccoli Bowl", 30, 4, "Cook rice. Brown beef in oil with garlic to 71 C. Add broccoli and 100 ml water, cover until tender and serve over rice.",
            (14, 250m, "g"), (17, 500m, "g"), (5, 1m, "tbsp"), (11, 2m, "cloves"), (23, 400m, "g"));
        Add(69, 7, "Beef Carrot Meatloaf", 60, 4, "Mix beef, grated carrot, finely chopped onion, egg and oats. Shape a loaf and bake at 180 C until the centre reaches 71 C.",
            (17, 600m, "g"), (12, 2m, "whole"), (10, 1m, "whole"), (2, 1m, "whole"), (28, 60m, "g"));
        Add(70, 7, "Beef Spinach Stuffed Tomatoes", 45, 4, "Hollow out tomatoes. Brown beef with onion to 71 C, stir in spinach, then fill tomatoes. Top with cheese and bake at 190 C for 20 minutes.",
            (9, 8m, "whole"), (17, 400m, "g"), (10, 1m, "whole"), (22, 150m, "g"), (20, 80m, "g"));

        Add(71, 8, "Lemon Baked Salmon", 25, 4, "Place salmon in a baking dish, drizzle with oil and lemon juice and season. Bake at 200 C until the centre reaches 63 C.",
            (18, 600m, "g"), (5, 1m, "tbsp"), (27, 1m, "whole"), (6, 0.5m, "tsp"));
        Add(72, 8, "Salmon Potato Tray Bake", 45, 4, "Toss diced potatoes in oil and roast at 200 C for 25 minutes. Add salmon and lemon juice and roast until salmon reaches 63 C.",
            (13, 700m, "g"), (5, 2m, "tbsp"), (18, 600m, "g"), (27, 1m, "whole"));
        Add(73, 8, "Salmon Broccoli Bake", 30, 4, "Place salmon and broccoli in an oiled baking dish. Add crushed garlic and bake at 200 C until broccoli is tender and salmon reaches 63 C.",
            (18, 600m, "g"), (23, 400m, "g"), (5, 2m, "tbsp"), (11, 2m, "cloves"));
        Add(74, 8, "Salmon Spinach Pasta", 30, 4, "Boil pasta. Cook diced salmon in butter to 63 C, add spinach and milk, simmer gently and toss with pasta.",
            (15, 350m, "g"), (18, 400m, "g"), (4, 20m, "g"), (22, 200m, "g"), (3, 150m, "ml"));
        Add(75, 8, "Tuna Potato Cakes", 35, 4, "Boil and mash potatoes, then mix with drained tuna and egg. Shape cakes, dust with flour and fry in oil until hot and egg is cooked through.",
            (13, 500m, "g"), (19, 300m, "g"), (2, 1m, "whole"), (1, 30m, "g"), (5, 2m, "tbsp"));
        Add(76, 8, "Tuna Stuffed Tomatoes", 15, 2, "Hollow out tomatoes and chop the removed flesh. Mix it with drained tuna, yogurt and finely chopped onion, then fill the tomatoes.",
            (9, 4m, "whole"), (19, 200m, "g"), (21, 80m, "g"), (10, 0.5m, "whole"));
        Add(77, 8, "Tuna Cheese Pasta Bake", 40, 4, "Boil pasta. Mix with drained tuna, chopped tomato and milk. Top with cheese and bake at 190 C for 25 minutes until bubbling.",
            (15, 350m, "g"), (19, 300m, "g"), (9, 4m, "whole"), (3, 150m, "ml"), (20, 150m, "g"));
        Add(78, 8, "Salmon Rice Bowl", 30, 2, "Cook rice and steam broccoli. Pan-fry salmon in oil until its centre reaches 63 C. Serve together with lemon juice.",
            (14, 150m, "g"), (23, 200m, "g"), (18, 300m, "g"), (5, 1m, "tbsp"), (27, 0.5m, "whole"));
        Add(79, 8, "Tuna Lentil Salad", 35, 4, "Boil lentils until tender, drain and cool. Toss with drained tuna, diced pepper, olive oil and lemon juice.",
            (29, 200m, "g"), (19, 300m, "g"), (25, 2m, "whole"), (5, 2m, "tbsp"), (27, 1m, "whole"));
        Add(80, 8, "Garlic Butter Salmon", 20, 2, "Melt butter with crushed garlic. Pan-fry salmon gently until the centre reaches 63 C, spooning butter over it. Finish with lemon juice.",
            (4, 25m, "g"), (11, 2m, "cloves"), (18, 300m, "g"), (27, 0.5m, "whole"));

        Add(81, 9, "Roasted Potato Wedges", 40, 4, "Cut potatoes into wedges, toss with oil, salt and pepper, and roast at 210 C for 35 minutes, turning halfway.",
            (13, 800m, "g"), (5, 2m, "tbsp"), (6, 0.5m, "tsp"), (7, 0.25m, "tsp"));
        Add(82, 9, "Broccoli Cheese Bake", 35, 4, "Steam broccoli until just tender. Make a sauce with butter, flour and milk, stir in cheese, pour over broccoli and bake at 190 C for 20 minutes.",
            (23, 600m, "g"), (4, 25m, "g"), (1, 25m, "g"), (3, 350m, "ml"), (20, 150m, "g"));
        Add(83, 9, "Zucchini Fritters", 25, 4, "Grate zucchini and squeeze dry. Mix with eggs, flour and cheese. Fry small patties in oil until golden and cooked through.",
            (26, 3m, "whole"), (2, 2m, "whole"), (1, 80m, "g"), (20, 60m, "g"), (5, 2m, "tbsp"));
        Add(84, 9, "Lentil Patties", 45, 4, "Boil lentils until tender and drain well. Mash with grated carrot, egg and oats. Shape patties and fry in oil until firm and cooked through.",
            (29, 250m, "g"), (12, 1m, "whole"), (2, 1m, "whole"), (28, 60m, "g"), (5, 2m, "tbsp"));
        Add(85, 9, "Spinach Stuffed Mushrooms", 30, 4, "Remove mushroom stems and chop them. Saute stems and spinach in oil. Fill caps, top with cheese and bake at 190 C for 20 minutes.",
            (24, 500m, "g"), (22, 200m, "g"), (5, 1m, "tbsp"), (20, 100m, "g"));
        Add(86, 9, "Vegetable Egg Bake", 35, 4, "Saute diced pepper and zucchini in oil. Place in a baking dish, pour over beaten eggs and milk, top with cheese and bake at 180 C until set.",
            (25, 2m, "whole"), (26, 1m, "whole"), (5, 1m, "tbsp"), (2, 6m, "whole"), (3, 100m, "ml"), (20, 100m, "g"));
        Add(87, 9, "Tomato Lentil Stew", 45, 4, "Saute onion and garlic in oil. Add lentils, chopped tomato and 700 ml water. Simmer until lentils are tender.",
            (10, 1m, "whole"), (11, 2m, "cloves"), (5, 1m, "tbsp"), (29, 250m, "g"), (9, 5m, "whole"));
        Add(88, 9, "Garlic Roasted Carrots", 35, 4, "Cut carrots into batons, toss with oil, crushed garlic and salt, and roast at 200 C for 30 minutes.",
            (12, 700m, "g"), (5, 2m, "tbsp"), (11, 2m, "cloves"), (6, 0.5m, "tsp"));
        Add(89, 9, "Cheese Stuffed Potatoes", 65, 4, "Bake potatoes at 200 C until tender. Halve and scoop out centres, mash with butter, milk and cheese, refill and bake for 10 minutes.",
            (13, 4m, "whole"), (4, 25m, "g"), (3, 80m, "ml"), (20, 120m, "g"));
        Add(90, 9, "Vegetable Tomato Skillet", 30, 4, "Saute onion and garlic in oil. Add diced zucchini, pepper and tomato, and simmer until all vegetables are tender.",
            (10, 1m, "whole"), (11, 2m, "cloves"), (5, 2m, "tbsp"), (26, 2m, "whole"), (25, 2m, "whole"), (9, 4m, "whole"));

        Add(91, 10, "Banana Oat Cookies", 25, 4, "Mash bananas and mix with oats and melted butter. Shape small cookies and bake at 180 C for 18 minutes.",
            (30, 2m, "whole"), (28, 150m, "g"), (4, 20m, "g"));
        Add(92, 10, "Rice Pudding", 40, 4, "Simmer rice in milk over low heat, stirring often, until soft. Stir in sugar and serve warm or chilled.",
            (14, 120m, "g"), (3, 800m, "ml"), (8, 60m, "g"));
        Add(93, 10, "Lemon Shortbread", 40, 8, "Cream butter and sugar, mix in flour and lemon zest. Press into a small tin and bake at 170 C for 25 minutes. Cool before slicing.",
            (4, 150m, "g"), (8, 75m, "g"), (1, 225m, "g"), (27, 1m, "whole"));
        Add(94, 10, "Banana Custard", 30, 4, "Whisk eggs, sugar and milk. Stir over low heat until custard reaches 71 C without boiling. Cool and serve with sliced banana.",
            (2, 3m, "whole"), (8, 50m, "g"), (3, 400m, "ml"), (30, 2m, "whole"));
        Add(95, 10, "Sweet Crepes", 25, 4, "Whisk flour, eggs, milk and sugar into a thin batter. Cook thin crepes in a buttered pan until golden on both sides.",
            (1, 150m, "g"), (2, 2m, "whole"), (3, 350m, "ml"), (8, 25m, "g"), (4, 20m, "g"));
        Add(96, 10, "Baked Banana Oats", 35, 4, "Mash bananas and mix with oats, milk and beaten egg. Pour into a buttered dish and bake at 180 C for 25 minutes until set.",
            (30, 2m, "whole"), (28, 200m, "g"), (3, 300m, "ml"), (2, 1m, "whole"), (4, 10m, "g"));
        Add(97, 10, "Lemon Yogurt Cups", 10, 4, "Mix yogurt with sugar, lemon juice and zest. Divide into cups and serve immediately or refrigerate.",
            (21, 500m, "g"), (8, 40m, "g"), (27, 1m, "whole"));
        Add(98, 10, "Butter Oat Bars", 35, 8, "Mix oats, flour and sugar with melted butter. Press into a lined tin and bake at 180 C for 25 minutes. Cool before cutting.",
            (28, 200m, "g"), (1, 100m, "g"), (8, 100m, "g"), (4, 150m, "g"));
        Add(99, 10, "Banana Yogurt Dessert", 5, 4, "Slice bananas and layer with yogurt in glasses. Sprinkle with oats and sugar just before serving.",
            (30, 3m, "whole"), (21, 400m, "g"), (28, 40m, "g"), (8, 20m, "g"));
        Add(100, 10, "Baked Egg Custard", 50, 4, "Whisk eggs, milk and sugar. Pour into ramekins in a hot water bath and bake at 160 C until just set and centres reach 71 C.",
            (2, 3m, "whole"), (3, 500m, "ml"), (8, 60m, "g"));

        modelBuilder.Entity<Recipe>().HasData(recipes);
        modelBuilder.Entity<RecipeIngredient>().HasData(recipeIngredients);

        void Add(int id, int categoryId, string name, int minutes, int servings, string instructions,
            params (int IngredientId, decimal Quantity, string Unit)[] ingredients)
        {
            recipes.Add(new Recipe
            {
                Id = id,
                CategoryId = categoryId,
                Name = name,
                Instructions = instructions,
                PreparationMinutes = minutes,
                Servings = servings
            });
            recipeIngredients.AddRange(ingredients.Select(ingredient => new RecipeIngredient
            {
                RecipeId = id,
                IngredientId = ingredient.IngredientId,
                Quantity = ingredient.Quantity,
                Unit = ingredient.Unit
            }));
        }
    }
}
