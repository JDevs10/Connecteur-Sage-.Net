using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Classes
{
    public class GenererCle
    {
        private static char[] Matrice1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        private static char[] Matrice2 = { 'k', 'E', 'u', 'H', 'A', 'a', 'G', 'l', 'C', 'b', 'X', 'w', 'V', 'v', 'O', 'M', 'q', 'T', '1', 'm', 'c', 'W', 'N', 'x', 'p', '4', '8', '9', 'd', 'R', 'Y', 'o', 'S', '2', 'n', 'r', 'y', 'Q', '7', 'e', 'P', 'j', '6', 'U', 'f', '0', 'Z', 'B', '5', 'I', 's', 'i', 'F', 'J', 'z', 'L', 'h', 'K', 't', 'D', '3', 'g' };


        private static char[] date1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '/', ':' };

        private static char[] date2 = { '6', 'I', 'V', 'Z', 'q', 'N', 'B', 'E', '2', 'M', '8', 'k' };

        private static char[] date3 = { 'T', 'C', 'X', 'L', 'D', 'F', 'J', 'K', 'A', 'G', 'U', 'W' };

        private static char[] date4 = { 'O', 'y', '5', '9', 'R', '0', 'Y', '1', 'S', '3', 'Q', 'r' };

        private static char[] date5 = { 'a', 't', 'u', 'o', 'b', 's', 'm', 'z', 'w', 'v', 'j', 'P' };

        private static char[] date6 = { 'c', 'f', 'p', 'h', 'x', 'g', 'n', 'i', 'e', 'l', 'd', '4' };

        private static Random random = new Random();

        private static string[] motcle = { "alONe", "azUR2", "A6roc", "Live7", "moRSe", "Sky92", "D4y1q", "I2g9z", "kAMhL", "LPmsd", "vu59J", "MD78s", "sMFrX", "Jn812", "bTFS6", "tHPSQ", "keBc9", "Si89d", "Pm5zN", "lIg2A" };

        private static string date = DateTime.Today.ToString().Replace(" 00:00:00", "");


        public static string Generer()
        {
              string crypterDate = "";

              string crypterMot = "";

            //______________________  crypterDate date ___________________________________________

            List<int> ListIndexDate = new List<int>();

            for (int i = 0; i < date.ToString().Length; i++)
            {
                for (int j = 0; j < date1.Length; j++)
                {
                    if (date1[j].ToString() == date.ToString().Substring(i, 1))
                    {
                        ListIndexDate.Add(j);
                    }
                }
            }

            for (int i = 0; i < ListIndexDate.Count; i++)
            {
                int ran = random.Next(2, 7);
                Console.WriteLine(ran);
                if (ran == 2)
                {
                    crypterDate = crypterDate + date2[ListIndexDate[i]];
                }
                if (ran == 3)
                {
                    crypterDate = crypterDate + date3[ListIndexDate[i]];
                }
                if (ran == 4)
                {
                    crypterDate = crypterDate + date4[ListIndexDate[i]];
                }
                if (ran == 5)
                {
                    crypterDate = crypterDate + date5[ListIndexDate[i]];
                }
                if (ran == 6)
                {
                    crypterDate = crypterDate + date6[ListIndexDate[i]];
                }
            }

            //_______________________ crypterDate mot cle __________________________________________

            string mot = motcle[random.Next(0, 20)];


            List<int> listIndexMotcle = new List<int>();

            for (int i = 0; i < mot.Length; i++)
            {
                for (int j = 0; j < Matrice1.Length; j++)
                {
                    if (Matrice1[j].ToString() == mot.Substring(i, 1))
                    {
                        listIndexMotcle.Add(j);
                    }
                }
            }

            for (int i = 0; i < listIndexMotcle.Count; i++)
            {
                crypterMot = crypterMot + Matrice2[listIndexMotcle[i]];
            }


            string cry = Matrice1[random.Next(0, 61)].ToString() + crypterMot + crypterDate;
            return cry.Substring(0, 4) + "-" + cry.Substring(4, 4) + "-" + cry.Substring(8, 4) + "-" + cry.Substring(12, 4);
        }


// ------------------------------------ decrypter la clé -------------------------------------------------------

        public static Boolean decrypter(string mot)
        {
            Boolean isValideDate = false;
            Boolean isValideMot = false;

            string crypterDate = mot.Substring(6,10);
            string decrypterDate = "";
            string crypterMot = mot.Substring(1, 5);
            string decrypterMot = "";

            List<int> ListIndexDate2 = new List<int>();

            for (int i = 0; i < crypterDate.Length; i++)
            {

                for (int j = 0; j < date2.Length; j++)
                {
                    if (crypterDate.ToString().Substring(i, 1) == date2[j].ToString())
                        ListIndexDate2.Add(j);
                    if (crypterDate.ToString().Substring(i, 1) == date3[j].ToString())
                        ListIndexDate2.Add(j);
                    if (crypterDate.ToString().Substring(i, 1) == date4[j].ToString())
                        ListIndexDate2.Add(j);
                    if (crypterDate.ToString().Substring(i, 1) == date5[j].ToString())
                        ListIndexDate2.Add(j);
                    if (crypterDate.ToString().Substring(i, 1) == date6[j].ToString())
                        ListIndexDate2.Add(j);
                }


            }

            for (int i = 0; i < ListIndexDate2.Count; i++)
            {
                decrypterDate = decrypterDate + date1[ListIndexDate2[i]];

            }



            DateTime dateTime;

            // tester si la date est valide
            if (DateTime.TryParse(decrypterDate, out dateTime))
            {
                if(dateTime == DateTime.Today)
                {
                    isValideDate = true;
                }
                else
                {
                    isValideDate = false;
                }
            }
            else
            {
                isValideDate = false;
            }

            //_______________________ decrypter mot ________________________________________________

            List<int> ListIndexMot2 = new List<int>();

            for (int i = 0; i < crypterMot.Length; i++)
            {

                for (int j = 0; j < Matrice2.Length; j++)
                {
                    if (crypterMot.ToString().Substring(i, 1) == Matrice2[j].ToString())
                        ListIndexMot2.Add(j);
                }


            }

            for (int i = 0; i < ListIndexMot2.Count; i++)
            {
                decrypterMot = decrypterMot + Matrice1[ListIndexMot2[i]];

            }

            for (int i = 0; i < motcle.Length; i++)
            {
                if (decrypterMot == motcle[i])
                {
                    isValideMot = true;
                }
            }

            if (isValideDate && isValideMot)
            {
                return true;
            }
            else
            {
                return false;
            }
            


        }
    }
}
