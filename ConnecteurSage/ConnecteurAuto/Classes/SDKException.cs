using System;
using System.Runtime.Serialization;

namespace ConnecteurAuto.Classes
{
  /// <summary>
    /// Classe représentant une exception propre à l'application Demo_SDK_GestionCommerciale
  /// </summary>
  [Serializable()]
  public class SDKException : Exception
  {
    #region Constructeurs
    /// <summary>
    /// Création d'une instance de SDKException
    /// </summary>
    public SDKException()
      : base()
    {
    }

    /// <summary>
    /// Création d'une instance de SDKException
    /// </summary>
    /// <param name="message">le message à retourner</param>
    public SDKException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Création d'une instance de SDKException
    /// </summary>
    /// <param name="message">le message à retourner</param>
    /// <param name="innerException">Exception enfant</param>
    public SDKException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// Création d'une instance de SDKException
    /// </summary>
    /// <param name="serializationInfo">Informations de serialisation</param>
    /// <param name="streamingContext">contexte</param>
    public SDKException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
    #endregion
  }
}
