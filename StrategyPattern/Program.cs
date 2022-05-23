using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the number of the month you would like to book a ticket for.");
            bool isInt = int.TryParse(Console.ReadLine(), out int month);

            while (month < 1 || month > 12 || !isInt)
            {
                Console.WriteLine("Please enter a correct month for your ticket.");
                isInt = int.TryParse(Console.ReadLine(), out month);
            }

            BookTicket ticket = new BookTicket(new StandardFee());

            if (month == 6 || month == 7)
            {
                ticket.FeeBehaviour = new Discount();
                ticket.Discount = 25;
            }
            else if (month == 12)
            {
                ticket.FeeBehaviour = new DoubledFee();
            }

            // If filePath doesn't work, check OrdersPath in App.config to make sure the path is set to your local machine.
            string filePath = ConfigurationManager.AppSettings["OrdersPath"];

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"{ticket.Fee} {month} {ticket.Discount} {ticket.Calculate()}");
            }

            Console.WriteLine($"Your ${ticket.Fee} has a discount of {ticket.Discount} for a total of {ticket.Calculate()}");

            GetOrders();

            Console.ReadKey(true);
        }

        public class BookTicket
        {
            public FeeCalculationBehaviour FeeBehaviour;
            public double Fee
            {
                get { return Double.Parse(ConfigurationManager.AppSettings["TicketFee"]); }
                set { Fee = value; }
            }
            public int Discount { get; set; } = 0;

            public BookTicket(FeeCalculationBehaviour fb)
            {
                FeeBehaviour = fb;
            }

            public double Calculate()
            {
                return FeeBehaviour.CalculateCost(Fee);
            }
        }

        public interface FeeCalculationBehaviour
        {
            double CalculateCost(double fee);
        }

        public class Discount : FeeCalculationBehaviour
        {
            public double CalculateCost(double fee)
            {
                double discount = fee * 0.25;
                double cost = fee - discount;
                return cost;
            }
        }

        public class DoubledFee : FeeCalculationBehaviour
        {
            public double CalculateCost(double fee)
            {
                double cost = fee * 2;
                return cost;
            }
        }

        public class StandardFee : FeeCalculationBehaviour
        {
            public double CalculateCost(double fee)
            {
                return fee;
            }
        }

        static void GetOrders()
        {
            Console.WriteLine("Displaying orders");
            string filePath = ConfigurationManager.AppSettings["OrdersPath"];
            FileStream stream = File.OpenRead(filePath);

            using (StreamReader sr = new StreamReader(stream))
            {
                string line = sr.ReadLine();

                while (line != null)
                {
                    Console.WriteLine(line);
                    line = sr.ReadLine();
                }
            }
        }
    }
}
