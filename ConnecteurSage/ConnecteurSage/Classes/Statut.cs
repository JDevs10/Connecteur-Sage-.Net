using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Classes
{
    public class Statut
    {
        public string Nom { get; set; }
        public int Value { get; set; }

        public Statut()
        {

        }

        public Statut(string Nom, int Value)
        {
            this.Nom = Nom;
            this.Value = Value;
        }

        public List<Statut> getListeStatut()
        {
            List<Statut> list = new List<Statut>();
            list.Add(new Statut("Saisie", 0));
            list.Add(new Statut("Confirmé", 1));
            list.Add(new Statut("Accepté", 2));
            return list;
        }

    }
}
