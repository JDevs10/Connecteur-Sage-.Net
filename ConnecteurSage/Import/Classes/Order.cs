﻿using System;
using System.Collections.Generic;

namespace Import.Classes
{
  /// <summary>
  /// Classe représentant une commande
  /// </summary>
  public class Order
  {
    #region Constructeurs
    /// <summary>
    /// Création d'une instance de Order
    /// </summary>
      public Order()
      {
          StockId = "0";
          deviseCommande = "0";
      }

        // DO_Piece, cli.CT_Num, Adresse, cmd.DO_DEVISE, cmd.DO_Date, cmd.DO_DateLivr, cmd.DO_Condition, cmd.DO_TotalHT, cli.CT_Intitule, cmd.DO_Motif, cli.CT_EdiCode, cmd.N_CATCOMPTA, liv.LI_Contact, cli.N_Expedition, cli.CT_Telephone, cli.CT_EMail, cli.CT_Commentaire

        public Order(string NumCommande, string codeClient, string adresseLivraison, string deviseCommande, string DateCommande, string DateLivraison, string conditionLivraison, string MontantTotal, string NomClient, string DO_MOTIF, string codeAcheteur, string codeFournisseur, string nom_contact)
      {
          this.NumCommande = NumCommande;
          this.codeClient = codeClient;
          this.adresseLivraison = adresseLivraison;
          this.deviseCommande = deviseCommande;
          this.DateCommande = DateCommande;
          this.DateLivraison = DateLivraison;
          this.conditionLivraison = conditionLivraison;
          this.MontantTotal = decimal.Parse(MontantTotal);
          this.NomClient = NomClient;
          this.DO_MOTIF = DO_MOTIF;
          this.codeFournisseur = codeFournisseur;
          this.codeAcheteur = codeAcheteur;
          this.nom_contact = nom_contact;
      }

    #endregion

    #region Propriétés
    /// <summary>
    /// Retourne et modifie l'identifiant de la commande
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Retourne et modifie le numero de la commande
    /// </summary>
    public string NumCommande { get; set; }
    /// <summary>
    /// Retourne et modifie le code client
    /// </summary>
    public string codeClient { get; set; }
    /// <summary>
    /// Retourne et modifie le code Fournisseur
    /// </summary>
    public string codeFournisseur { get; set; }
    /// <summary>
    /// Retourne et modifie le code Acheteur
    /// </summary>
    public string codeAcheteur { get; set; }
    /// <summary>
    /// Retourne et modifie de la date de la livraison
    /// </summary>
    public string adresseLivraison { get; set; }
    /// <summary>
    /// Retourne et modifie de la devise du Commande
    /// </summary>
    public string deviseCommande { get; set; }
    /// <summary>
    /// Retourne et modifie de la date de la commande
    /// </summary>  
    public string DateCommande { get; set; }
    /// </summary>
    /// Retourne et modifie date livraison
    /// </summary>
    public string DateLivraison { get; set; }
    /// <summary>
    /// Retourne et modifie le Montant Total
    /// </summary>
    /// 
    public decimal MontantTotal { get; set; }
    /// <summary>
    /// id de stock
    /// </summary>
    public string StockId { get; set; }
    /// <summary>
    /// Retourne et modifie le Montant Total
    /// </summary>
    public string conditionLivraison { get; set; }
    /// <summary>
    /// Reference
    /// </summary>
    public string Reference { get; set; }
    /// <summary>
    /// commentaires
    /// </summary>
    public string commentaires { get; set; }
    /// <summary>
    /// Liste ligne
    /// </summary>
    public List<OrderLine> Lines { get; set; }
    public string nom_contact { get; set; }
    public string adresse { get; set; }
    public string adresse_2 { get; set; }
    public string codepostale { get; set; }
    public string ville { get; set; }
    public string pays { get; set; }
    public string do_coord01 { get; set; }
    public string DO_MOTIF { get; set; }
    public string NomClient { get; set; }
    public string cbMarq { get; set; }
    public string statut { get; set; }
    public string telephone { get; set; }
    public string email { get; set; }
    public string HeureCommande { get; set; }
    public string HeureLivraison { get; set; }
    public string Transporteur { get; set; }

        #endregion


    }

  /// <summary>
  /// Classe repésentant une ligne de vente de la commande
  /// </summary>
  public class OrderLine
  {
    #region Constructeurs
    /// <summary>
    /// Création d'une instance de OrderLine
    /// </summary>
    public OrderLine()
    {
        Quantite = "1";
    }

    public OrderLine(string NumLigne,
string codeArticle,
string descriptionArticle,
string Quantite,
string PrixNetHT,
string MontantLigne,
string DateLivraison,
string codeFournis,
string codeAcheteur)
    {
        this.NumLigne = NumLigne;
        this.codeArticle = codeArticle;
        this.descriptionArticle = descriptionArticle;
        this.Quantite = Quantite;
        this.PrixNetHT = PrixNetHT;
        this.MontantLigne = MontantLigne;
        this.DateLivraison = DateLivraison;
        this.codeFournis = codeFournis;
        this.codeAcheteur = codeAcheteur;
    }
    #endregion

    #region Propriétés
    /// <summary>
    /// Retourne et modifie le Numero de ligne
    /// </summary>
    public string NumLigne { get; set; }
    /// <summary>
    /// Retourne et modifie le code d'article
    /// </summary>
    public string codeArticle { get; set; }
    /// <summary>
    /// Retourne et modifie la Quantité
    /// </summary>
    public string descriptionArticle { get; set; }
    /// <summary>
    /// Retourne et modifie la Quantité
    /// </summary>
    public string Quantite { get; set; }
    /// <summary>
    /// Retourne et modifie le prix de l'article
    /// </summary>
    public string PrixNetHT { get; set; }
    /// <summary>
    /// Retourne et modifie le Montant Ligne
    /// </summary>
    public string MontantLigne { get; set; }
    /// <summary>
    /// Retourne et modifie DateLivraison
    /// </summary>
    public string DateLivraison { get; set; }

    public string codeAcheteur { get; set; }

    public string codeFournis { get; set; }

    public string Calcule_conditionnement = "0";

    public Article article { get; set; }

    public string DO_Ref { get; set; }
    public string DL_QteBC { get; set; }
    public string DL_PieceBC { get; set; }
    public string DL_QteDE { get; set; }
    public string DL_QtePL { get; set; }
    public string DL_DateBC { get; set; }
    public string DL_DateDE { get; set; }
    public string DL_DatePL { get; set; }
    public string DL_MontantHT { get; set; }
    public string DL_MontantTTC { get; set; }
        #endregion
    }
}
