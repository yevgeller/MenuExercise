using Newtonsoft.Json.Linq;

Console.WriteLine("Hello, World!");
//here's how I solved this: https://gist.github.com/yevgeller/7521a06298ce01758739a0b4b8c59a4c

//download the file
HttpClient client = new HttpClient();
var menu = client.GetStringAsync("https://yevgellerdesignpatterns.azurewebsites.net/static/menu.json").Result;

var obj = JObject.Parse(menu);

var items = obj["menuItems"] as IEnumerable<dynamic>;

//var foodTypes = items.Select(x => x["foodType"]).Distinct();

//get 5 cheapest appetizers

var appetizers = items
    .Where(x => x["foodType"] == "appetizer")
    .OrderBy(item => (double)item["price"])
    .Take(5);

//get 5 desserts 
var desserts = items
    .Where(x => x["foodType"] == "dessert")
    .OrderBy(item => (double)item["price"])
    .Take(5);


//see how much it is
var interimTotal = appetizers.Sum(a => (double)a["price"]) + 
    desserts.Sum(d => (double)d["price"]);
//spend the rest on entrees
var remainingTotal = 300 - interimTotal;

//entrees in asc by price and see which ones we can get until we run out of money
var entrees = items
    .Where(x => x["foodType"] == "entree")
    .OrderBy(item => (double)item["price"])
    .TakeWhile(x => (remainingTotal -= (double)x["price"]) > 0);

Console.WriteLine($"Total number of items that can be ordered: {appetizers.Count() + desserts.Count() + entrees.Count()}");

Console.WriteLine("Final Order:");
Console.WriteLine("----------------------");
Console.WriteLine("Appetizers:");
appetizers.ToList().ForEach(x =>
{
    Console.WriteLine($"{x["name"]} at ${x["price"]})");
});
Console.WriteLine("Entrees:");
entrees.ToList().ForEach(x =>
{
    Console.WriteLine($"{x["name"]} at ${x["price"]})");
});
Console.WriteLine("Desserts:");
desserts.ToList().ForEach(x =>
{
    Console.WriteLine($"{x["name"]} at ${x["price"]})");
});
Console.WriteLine("Remaning funds: $" + remainingTotal);