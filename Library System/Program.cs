using System;
using System.Linq;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TotalQuantity { get; set; }
    public int TotalBorrowed { get; set; }

    public Book()
    {
        TotalQuantity = TotalBorrowed = 0;
        Id = -1;
        Name = "";
    }

    public void Read()
    {
        Console.WriteLine("Enter book info: id & name & total quantity:");
        string[] input = Console.ReadLine().Split();
        Id = int.Parse(input[0]);
        Name = input[1];
        TotalQuantity = int.Parse(input[2]);
        TotalBorrowed = 0;
    }

    public bool Borrow(int userId)
    {
        if (TotalQuantity - TotalBorrowed == 0)
            return false;
        ++TotalBorrowed;
        return true;
    }

    public void ReturnCopy()
    {
        if (TotalBorrowed > 0)
            --TotalBorrowed;
    }

    public bool HasPrefix(string prefix)
    {
        if (Name.Length < prefix.Length)
            return false;

        return Name.StartsWith(prefix);
    }

    public void Print()
    {
        Console.WriteLine($"id = {Id} name = {Name} total_quantity = {TotalQuantity} total_borrowed = {TotalBorrowed}");
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int[] BorrowedBooksIds { get; set; }
    public int Len { get; set; }

    public User()
    {
        Name = "";
        Len = 0;
        Id = -1;
        BorrowedBooksIds = new int[LibrarySystem.MAX_BOOKS];
    }

    public void Read()
    {
        Console.WriteLine("Enter user name & national id:");
        string[] input = Console.ReadLine().Split();
        Name = input[0];
        Id = int.Parse(input[1]);
    }

    public void Borrow(int bookId)
    {
        BorrowedBooksIds[Len++] = bookId;
    }

    public void ReturnCopy(int bookId)
    {
        bool removed = false;
        for (int i = 0; i < Len; ++i)
        {
            if (BorrowedBooksIds[i] == bookId)
            {
                for (int j = i + 1; j < Len; ++j)
                    BorrowedBooksIds[j - 1] = BorrowedBooksIds[j];
                removed = true;
                --Len;
                break;
            }
        }
        if (!removed)
            Console.WriteLine($"User {Name} never borrowed book id {bookId}");
    }

    public bool IsBorrowed(int bookId)
    {
        return BorrowedBooksIds.Contains(bookId);
    }

    public void Print()
    {
        Array.Sort(BorrowedBooksIds, 0, Len);
        Console.Write($"user {Name} id {Id} borrowed books ids: ");
        for (int i = 0; i < Len; ++i)
            Console.Write($"{BorrowedBooksIds[i]} ");
        Console.WriteLine();
    }
}

public class LibrarySystem
{
    public const int MAX_BOOKS = 10;
    public const int MAX_USERS = 10;

    private int totalBooks;
    private Book[] books;
    private int totalUsers;
    private User[] users;

    public LibrarySystem()
    {
        totalBooks = totalUsers = 0;
        books = new Book[MAX_BOOKS];
        users = new User[MAX_USERS];
    }

    public void Run()
    {
        while (true)
        {
            int choice = Menu();

            if (choice == 1)
                AddBook();
            else if (choice == 2)
                SearchBooksByPrefix();
            else if (choice == 3)
                PrintWhoBorrowedBookByName();
            else if (choice == 4)
                PrintLibraryById();
            else if (choice == 5)
                PrintLibraryByName();
            else if (choice == 6)
                AddUser();
            else if (choice == 7)
                UserBorrowBook();
            else if (choice == 8)
                UserReturnBook();
            else if (choice == 9)
                PrintUsers();
            else
                break;
        }
    }

    private int Menu()
    {
        int choice = -1;
        while (choice == -1)
        {
            Console.WriteLine("\nEnter your menu choice [1 - 10]: ");
            if (!(int.TryParse(Console.ReadLine(), out choice) && 1 <= choice && choice <= 10))
            {
                Console.WriteLine("Invalid choice. Try again");
                choice = -1;
            }
        }
        return choice;
    }

    private void AddBook()
    {
        books[totalBooks++] = new Book();
        books[totalBooks - 1].Read();
    }

    private void SearchBooksByPrefix()
    {
        Console.WriteLine("Enter book name prefix: ");
        string prefix = Console.ReadLine();

        int cnt = 0;
        for (int i = 0; i < totalBooks; ++i)
        {
            if (books[i].HasPrefix(prefix))
            {
                Console.WriteLine(books[i].Name);
                ++cnt;
            }
        }

        if (cnt == 0)
            Console.WriteLine("No books with such prefix");
    }

    private void AddUser()
    {
        users[totalUsers++] = new User();
        users[totalUsers - 1].Read();
    }

    private int FindBookIdxByName(string name)
    {
        for (int i = 0; i < totalBooks; ++i)
        {
            if (name == books[i].Name)
                return i;
        }
        return -1;
    }

    private int FindUserIdxByName(string name)
    {
        for (int i = 0; i < totalUsers; ++i)
        {
            if (name == users[i].Name)
                return i;
        }
        return -1;
    }

    private bool ReadUserNameAndBookName(out int userIdx, out int bookIdx, int trials = 3)
    {
        userIdx = bookIdx = -1;
        string userName, bookName;

        while (trials-- > 0)
        {
            Console.WriteLine("Enter user name and book name: ");
            string[] input = Console.ReadLine().Split();
            userName = input[0];
            bookName = input[1];

            userIdx = FindUserIdxByName(userName);

            if (userIdx == -1)
            {
                Console.WriteLine("Invalid user name. Try again");
                continue;
            }

            bookIdx = FindBookIdxByName(bookName);

            if (bookIdx == -1)
            {
                Console.WriteLine("Invalid book name. Try again");
                continue;
            }

            return true;
        }

        Console.WriteLine("You did several trials! Try later.");
        return false;
    }

    private void UserBorrowBook()
    {
        int userIdx, bookIdx;

        if (!ReadUserNameAndBookName(out userIdx, out bookIdx))
            return;

        int userId = users[userIdx].Id;
        int bookId = books[bookIdx].Id;

        if (!books[bookIdx].Borrow(userId))
            Console.WriteLine("No more copies available right now");
        else
            users[userIdx].Borrow(bookId);
    }

    private void UserReturnBook()
    {
        int userIdx, bookIdx;

        if (!ReadUserNameAndBookName(out userIdx, out bookIdx))
            return;

        int bookId = books[bookIdx].Id;
        books[bookIdx].ReturnCopy();
        users[userIdx].ReturnCopy(bookId);
    }

    private void PrintLibraryById()
    {
        Array.Sort(books, (x, y) => x.Id.CompareTo(y.Id));

        Console.WriteLine();
        foreach (var book in books)
        {
            if (book != null)
                book.Print();
        }
    }

    private void PrintLibraryByName()
    {
        Array.Sort(books, (x, y) => string.Compare(x.Name, y.Name));

        Console.WriteLine();
        foreach (var book in books)
        {
            if (book != null)
                book.Print();
        }
    }

    private void PrintUsers()
    {
        Console.WriteLine();
        foreach (var user in users)
        {
            if (user != null)
                user.Print();
        }
    }

    private void PrintWhoBorrowedBookByName()
    {
        Console.WriteLine("Enter book name: ");
        string bookName = Console.ReadLine();

        int bookIdx = FindBookIdxByName(bookName);

        if (bookIdx == -1)
        {
            Console.WriteLine("Invalid book name.");
            return;
        }

        int bookId = books[bookIdx].Id;

        if (books[bookIdx].TotalBorrowed == 0)
        {
            Console.WriteLine("No borrowed copies");
            return;
        }

        foreach (var user in users)
        {
            if (user != null && user.IsBorrowed(bookId))
                Console.WriteLine(user.Name);
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        LibrarySystem library = new LibrarySystem();
        library.Run();
    }
}
