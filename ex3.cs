using System;
using Internal;

public class BankAccount
{
    // Private fields
    private decimal _balance;
    private string _pin;
    private int _failedAttempts;

    // Read-only property
    public string AccountHolder { get; }

    // Property with private setter
    public bool IsLocked { get; private set; }

    // Constructor
    public BankAccount(string accountHolder, decimal initialBalance, string initialPin)
    {
        AccountHolder = accountHolder;
        _balance = initialBalance > 0 ? initialBalance : 0;
        _pin = initialPin;
        _failedAttempts = 0;
        IsLocked = false;
    }

    // Deposit
    public bool Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Error: Deposit amount must be positive.");
            return false;
        }

        _balance += amount;

        Console.WriteLine($"Successfully deposited {amount:C}.");
        return true;
    }

    // Withdraw
    public bool Withdraw(decimal amount, string inputPin)
    {
        // Check if account is locked
        if (IsLocked)
        {
            Console.WriteLine("Error: Account is locked due to multiple failed PIN attempts.");
            return false;
        }

        // Check PIN
        if (inputPin != _pin)
        {
            _failedAttempts++;

            if (_failedAttempts >= 3)
            {
                IsLocked = true;
                Console.WriteLine("Error: Invalid PIN code. Account has been LOCKED for security!");
            }
            else
            {
                Console.WriteLine($"Error: Invalid PIN code. (Attempt {_failedAttempts}/3)");
            }

            return false;
        }

        // Correct PIN
        _failedAttempts = 0;

        // Check amount
        if (amount <= 0)
        {
            Console.WriteLine("Error: Withdrawal amount must be positive.");
            return false;
        }

        // Check balance
        if (_balance < amount)
        {
            Console.WriteLine("Error: Insufficient balance.");
            return false;
        }

        // Withdraw
        _balance -= amount;

        Console.WriteLine($"Successfully withdrew {amount:C}.");
        return true;
    }

    // Get Balance
    public decimal GetBalance(string inputPin)
    {
        if (inputPin != _pin)
        {
            Console.WriteLine("Error: Invalid PIN code.");
            return -1m;
        }

        return _balance;
    }

    // Change PIN
    public bool ChangePin(string currentPin, string newPin)
    {
        // Check current PIN
        if (currentPin != _pin)
        {
            Console.WriteLine("Error: Invalid current PIN.");
            return false;
        }

        // Check new PIN
        if (string.IsNullOrEmpty(newPin) ||
            newPin.Length != 4 ||
            !IsNumeric(newPin))
        {
            Console.WriteLine("Error: New PIN must be exactly 4 numeric digits.");
            return false;
        }

        // Change PIN
        _pin = newPin;

        Console.WriteLine("PIN changed successfully.");
        return true;
    }

    // Check if PIN contains only numbers
    private bool IsNumeric(string value)
    {
        foreach (char c in value)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        BankAccount account = new BankAccount("John Doe", 500.00m, "1234");

        Console.WriteLine($"Account Holder: {account.AccountHolder}");

        // Direct field access is impossible!
        // account._balance = 1000000m;
        // account._pin = "0000";

        Console.WriteLine("\n--- 1. Testing Deposit ---");
        account.Deposit(-50m);
        account.Deposit(200m);

        Console.WriteLine("\n--- 2. Testing Protected Balance View ---");
        account.GetBalance("9999");

        decimal currentBalance = account.GetBalance("1234");
        Console.WriteLine($"Verified Balance: {currentBalance:C}");

        Console.WriteLine("\n--- 3. Testing Lockout Mechanism ---");
        account.Withdraw(100m, "0000");
        account.Withdraw(100m, "1111");
        account.Withdraw(100m, "2222");

        // Further attempts should fail immediately due to lock
        account.Withdraw(100m, "1234");

        Console.WriteLine("\n--- 4. Account Lock Status ---");
        Console.WriteLine($"Is account locked? {account.IsLocked}");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
   
}