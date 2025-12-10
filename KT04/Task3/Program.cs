using System;
using System.Reflection;

class Book
{
    public string Name { get; set;}
    public string Title { get; set;}
    public string Autor { get; set;}

    public Book()
    {
        Name = "Papa";
        Title = "Xp";
        Autor = "Ya";
    }

    public Book(string name, string title)
    {
        Name = name;
        Title = title;
        Autor = "Ya";
    }

    public Book(string name, string title, string autor)
    {
        Name = name;    
        Title = title;
        Autor = autor;
    }

    public override string ToString()
    {
        return $"{Name}, {Title}, {Autor}";
    }

    class Programm
    {
        static void Main(string[] args)
        {
        Type bookType = typeof(Book);

        ConstructorInfo[] constructors = bookType.GetConstructors(
            BindingFlags.Public | 
            BindingFlags.Instance | 
            BindingFlags.DeclaredOnly
        );


        for (int i = 0; i < constructors.Length; i++)
        {
            ConstructorInfo constructor = constructors[i];
            Console.WriteLine($"Конструктор #{i + 1}:");

            ParameterInfo[] parameters = constructor.GetParameters();
            
            if (parameters.Length > 0)
            {
                Console.WriteLine("  Параметры:");
                foreach (var param in parameters)
                {
                    Console.WriteLine($"    - {param.ParameterType.Name} {param.Name}");
                }
            }
            Console.WriteLine();
        }

        
        }
    }
}

