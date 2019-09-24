using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnecteurSage.Classes
{
    class DocumentLine
    {
        public string domaine;
        public string type;
        public string piece;
        public string date;
        public string dateBL;
        public string ligne;
        public string ref_DO;
        public string ref_AR;
        public string design;
        public string quantity_DL;
        public string poidsNet;
        public string poidsBrut;
        public string prixUnitaire;
        public string prixRu;
        public string cmup;
        public string quantity_EU;
        public string montantHT;
        public string montantTTC;
        public string mum;

        public DocumentLine()
        {
        }

        public DocumentLine(string domaine, string type, string piece, string date, string dateBL, string ligne, string ref_DO, string ref_AR, string design, string quantity_DL, 
                            string poidsNet, string poidsBrut, string prixUnitaire, string prixRu, string cmup, string quantity_EU, string montantHT, string montantTTC, string mum)
        {
            this.domaine = domaine;
            this.type = type;
            this.piece = piece;
            this.date = date;
            this.dateBL = dateBL;
            this.ligne = ligne;
            this.ref_DO = ref_DO;
            this.ref_AR = ref_AR;
            this.design = design;
            this.quantity_DL = quantity_DL;
            this.poidsNet = poidsNet;
            this.poidsBrut = poidsBrut;
            this.prixUnitaire = prixUnitaire;
            this.prixRu = prixRu;
            this.cmup = cmup;
            this.quantity_EU = quantity_EU;
            this.montantHT = montantHT;
            this.montantTTC = montantTTC;
            this.mum = mum;
        }

        public void setDomaine(string domaine){ this.domaine = domaine;}
        public string getDomaine() { return this.domaine; }

        public void setType(string type) { this.type = type; }
        public string getType() { return this.type; }

        public void setPiece(string piece) { this.piece = piece; }
        public string getPiece() { return this.piece; }

        public void setDate(string date) { this.date = date; }
        public string getDate() { return this.date; }

        public void setDateBL(string dateBL) { this.dateBL = dateBL; }
        public string getDateBL() { return this.dateBL; }

        public void setLigne(string ligne) { this.ligne = ligne; }
        public string getLigne() { return this.ligne; }

        public void setRef_DO(string ref_DO) { this.ref_DO = ref_DO; }
        public string getRef_DO() { return this.ref_DO; }

        public void setRef_AR(string ref_AR) { this.ref_AR = ref_AR; }
        public string getRef_AR() { return this.ref_AR; }

        public void setDesign(string design) { this.design = design; }
        public string getDesign() { return this.design; }

        public void setQuantity_DL(string quantity_DL) { this.quantity_DL = quantity_DL; }
        public string getQuantity_DL() { return this.quantity_DL; }

        public void setPoidsNet(string poidsNet) { this.poidsNet = poidsNet; }
        public string getPoidsNet() { return this.poidsNet; }

        public void setPoidsBrut(string poidsBrut) { this.poidsBrut = poidsBrut; }
        public string getPoidsBrut() { return this.poidsBrut; }

        public void setPrixUnitaire(string prixUnitaire) { this.prixUnitaire = prixUnitaire; }
        public string getPrixUnitaire() { return this.prixUnitaire; }

        public void setPrixRu(string prixRu) { this.prixRu = prixRu; }
        public string getPrixRu() { return this.prixRu; }

        public void setCmup(string cmup) { this.cmup = cmup; }
        public string getCmup() { return this.cmup; }

        public void setQuantity_EU(string quantity_EU) { this.quantity_EU = quantity_EU; }
        public string getQuantity_EU() { return this.quantity_EU; }

        public void setMontantHT(string montantHT) { this.montantHT = montantHT; }
        public string getMontantHT() { return this.montantHT; }

        public void setMontantTTC(string montantTTC) { this.montantTTC = montantTTC; }
        public string getMontantTTC() { return this.montantTTC; }

        public void setMum(string mum) { this.mum = mum; }
        public string getMum() { return this.mum; }
    }
}
