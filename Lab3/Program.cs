var list = new SimpleList();
list.Add("first");
list.Add("second");
list.Add("third");

Console.WriteLine("SimpleList:");
foreach (var item in list)
{
    Console.WriteLine($"  {item}");
}

var dict = new SimpleDictionary<string, int>();
dict["one"] = 1;
dict["two"] = 2;
dict["three"] = 3;

Console.WriteLine("\nSimpleDictionary:");
foreach (var kvp in dict)
{
    Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
}

var linkedList = new BidirectionalLinkedList();
linkedList.Add("a");
linkedList.Add("b");
linkedList.Add("c");

Console.WriteLine("\nBidirectionalLinkedList:");
foreach (var item in linkedList)
{
    Console.WriteLine($"  {item}");
}
