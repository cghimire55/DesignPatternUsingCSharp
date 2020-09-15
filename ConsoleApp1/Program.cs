using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.Service;
using ConsoleApp1.Model;

namespace ConsoleApp1
{
    class Program
    {
        bool isDone;
        static void Main(string[] args)
        {

            //Delegate and Event
            //EncodeVideoService encodeVideoService = new EncodeVideoService();
            //SendEmailService sendEmailService = new SendEmailService();
            //encodeVideoService.VideoEncoded += sendEmailService.SendEmail;
            //encodeVideoService.EncodeVideo(new Video { Title = "my Title" });

            //Liskov Substution Principle
            //Fruit f = new Apple();
            //Console.WriteLine(f.GetColor());
            //f = new Orange();
            //Console.WriteLine(f.GetColor());

            //Open/Close principle

            //  Invoice i = new Invoice();
            //InvoiceDiscount invoiceDiscount = new InvoiceDiscount();
            //FinalDiscount finalDiscount = new FinalDiscount();
            //using(var i=new Invoice())
            //{
            //    Console.WriteLine(i.GetDiscount(1000));
            //}

            //Console.WriteLine(invoiceDiscount.GetDiscount(1000));
            //Console.WriteLine(finalDiscount.GetDiscount(1000));


            //threading
            //Thread t = new Thread(WriteY);
            //t.Start();
            //for (int i = 0; i < 500; i++)
            //{
            //    Console.WriteLine("x");
            //}
            //new Thread(Print).Start();
            //for (int j = 101; j <= 200; j++)
            //{
            //    Console.WriteLine(j);
            //}
            AbstractTest abstractTest = new myClass(); Console.WriteLine("Name= "+abstractTest.GetName()+" Address= "+abstractTest.GetAddress());
            Console.ReadLine();
        }
        private static void   Print()
        {
            for (int i = 1; i <= 100; i++)
            {
                Console.WriteLine(i);
            }
        }
      
        
    }
    #region Liskov Substution Principle
    public abstract class Fruit
    {
        public abstract string GetColor();
    }
    public class Apple : Fruit
    {
        public override string GetColor()
        {
            return "Red";
        }
    }
    public class Orange : Fruit
    {
        public override string GetColor()
        {
            return "Orange";
        }
    }
    #endregion
    #region Open/Close Principle
    public class Invoice:IDisposable
    {
        public virtual decimal GetDiscount(decimal amount)
        {
            return amount;
        }
        public void Dispose()
        {
            this.Dispose();
        }
    }
    public class InvoiceDiscount:Invoice
    {
        public override decimal GetDiscount(decimal amount)
        {
            return base.GetDiscount(amount) - 40;
        }
    }
    public class FinalDiscount : Invoice
    {
        public override decimal GetDiscount(decimal amount)
        {
            return base.GetDiscount(amount) - 30;
        }
    }
    #endregion

    public abstract class AbstractTest
    {
        private string Name = null;
        public AbstractTest()
        {
            Name = "Pramod";
        }
        public  string GetName()
        {
            if (Name==null)
            {
                return "Chandra";
            }
            return Name;
        }
        public abstract string GetAddress();
    }
    public class myClass:AbstractTest
    {
        public override string GetAddress()
        {
            return "Dhangadhi";
        }
    }

}
