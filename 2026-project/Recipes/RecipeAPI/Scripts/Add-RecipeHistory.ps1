param(
    [string]$BaseUrl = 'http://localhost:5085',
    [ValidateRange(1, [int]::MaxValue)]
    [int]$RecipeId = 1
)

$ErrorActionPreference = 'Stop'
$uri = "$($BaseUrl.TrimEnd('/'))/api/recipes/$RecipeId"
$recipe = Invoke-RestMethod -Uri $uri
$before = @(Invoke-RestMethod -Uri "$uri/history?mode=all").Count
if ($recipe.preparationMinutes -gt ([int]::MaxValue - 20)) {
    throw 'Preparation minutes cannot safely be increased twenty times.'
}

$initialMinutes = $recipe.preparationMinutes
for ($update = 1; $update -le 20; $update++) {
    $body = @{
        name = $recipe.name
        description = $recipe.description
        instructions = $recipe.instructions
        preparationMinutes = $initialMinutes + $update
        servings = $recipe.servings
        categoryId = $recipe.categoryId
        recipeIngredients = @($recipe.recipeIngredients | ForEach-Object {
            @{ ingredientId = $_.ingredientId; quantity = $_.quantity; unit = $_.unit }
        })
    } | ConvertTo-Json -Depth 5
    Invoke-RestMethod -Uri $uri -Method Put -ContentType 'application/json' -Body $body | Out-Null
    Start-Sleep -Milliseconds 100
}

$history = @(Invoke-RestMethod -Uri "$uri/history?mode=all")
if ($history.Count -ne ($before + 20)) {
    throw "Expected $($before + 20) versions, found $($history.Count). Check for concurrent writes."
}
Write-Output "Updated '$($recipe.name)' (ID $RecipeId) twenty times; $($history.Count) versions now exist."
Write-Output "Preparation minutes changed from $initialMinutes to $($initialMinutes + 20)."
Write-Output "History: /recipes/$RecipeId/history"
