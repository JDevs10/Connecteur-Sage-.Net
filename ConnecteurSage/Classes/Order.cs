using System;
using System.Collections.Generic;

namespace ConnecteurSage.Classes
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

      public Order(string NumCommande, string codeClient, string adresseLivraison, string deviseCommande, string DateCommande, string DateLivraison, string conditionLivraison, string MontantTotal, string NomClient)
      {
          this.NumCommande = NumCommande;
          this.codeClient = codeClient;
          this.adresseLivraison = adresseLivraison;
          this.deviseCommande = deviseCommande;
          this.DateCommande = DateCommande;
          this.DateLivraison = DateLivraison;
          this.conditionLivraison = conditionLivraison;
          this.MontantTotal = MontantTotal;
          this.NomClient = NomClient;
      }

      public Order(string NumCommande, string codeClient, string adresseLivraison, string deviseCommande, string DateCommande, string DateLivraison, string conditionLivraison, string MontantTotal, string NomClient, string DO_MOTIF, string codeAcheteur, string codeFournisseur, string nom_contact)
      {
          this.NumCommande = NumCommande;
          this.codeClient = codeClient;
          this.adresseLivraison = adresseLivraison;
          this.deviseCommande = deviseCommande;
          this.DateCommande = DateCommande;
          this.DateLivraison = DateLivraison;
          this.conditionLivraison = conditionLivraison;
          this.MontantTotal = MontantTotal;
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
    // Numero de commande enregistrer dans do_motif
    /// </summary>
    public string DO_MOTIF { get; set; }
    /// <summary>
    /// Retourne et modifie le code client
    /// </summary>
    public string codeClient { get; set; }
    /// <summary>
    /// Retourne et modifie le code Acheteur
    /// </summary>
    public string codeAcheteur { get; set; }
    /// <summary>
    /// Retourne et modifie le code Fournisseur
    /// </summary>
    public string NomClient { get; set; }
    /// <summary>
    /// Retourne et modifie le code Fournisseur
    /// </summary>
    public string codeFournisseur { get; set; }
    /// <summary>
    /// Retourne et modifie de l'adresse de livraison
    /// </summary>
    public string villeReference { get; set; }
    /// <summary>
    /// Retourne et modifie de l'adresse de livraison
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
    /// <summary>
    /// Retourne et modifie le Montant Total
    /// </summary>
    public string MontantTotal { get; set; }
    /// <summary>
    /// Retourne et modifie le nom du tiers rattaché à la commande
    /// </summary>
    public string StockId { get; set; }
    /// <summary>
    /// Retourne et modifie le nom du tiers rattaché à la commande
    /// </summary>
    public string DateLivraison { get; set; }
    /// <summary>
    /// Retourne et modifie le Montant Total
    /// </summary>
    public string conditionLivraison { get; set; }
    /// <summary>
    /// Reference
    /// </summary>
    public string Reference { get; set; }
    /// <summary>
    /// Liste ligne
    /// </summary>
    public string commentaires { get; set; }
    /// <summary>
    /// Liste ligne
    /// </summary>
    public List<OrderLine> Lines { get; set; }
    public string nom_contact { get; set; }
    public string adresse { get; set; }
    public string codepostale { get; set; }
    public string ville { get; set; }
    public string pays { get; set; }
    public string do_coord01 { get; set; }
    public decimal FNT_MONTANTTOTALTAXES { get; set; }
    public decimal DL_MONTANTHT { get; set; }
    public decimal DL_MONTANTTTC { get; set; }
   

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
    public string Prix { get; set; }
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



    #endregion
  }
}
