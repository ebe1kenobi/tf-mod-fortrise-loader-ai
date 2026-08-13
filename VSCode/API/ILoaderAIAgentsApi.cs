namespace TFModFortRiseLoaderAI;

/// <summary>
/// Qui pilote un emplacement : un humain, ou l'un des agents installes.
///
/// Interface SEPAREE de <see cref="ILoaderAIModApi"/>, et pas par gout du rangement :
/// l'interop de FortRise construit son proxy sur la FORME des membres. Un appelant qui
/// declare un membre absent de la version installee n'obtient plus rien du tout - pas
/// meme les membres qui existent. Ajouter ces methodes a l'interface d'enregistrement
/// aurait donc empeche tous les mods d'IA existants de s'enregistrer.
///
/// Demandee a part, une version anterieure de ce mod rend simplement null, et
/// l'appelant se contente de ne pas proposer le choix.
/// </summary>
public partial interface ILoaderAIAgentsApi
{
  /// <summary>
  /// Les types d'agents enregistres, dans l'ordre ou les proposer. Jamais null, vide
  /// quand aucun mod d'IA n'est installe.
  ///
  /// Ce sont les chaines qu'un profil enregistre - "Jimmy" aujourd'hui. Un type ajoute
  /// plus tard, ou un meme mod publiant plusieurs niveaux de difficulte, apparaitra ici
  /// sans que l'appelant change.
  /// </summary>
  string[] GetAgentTypes();

  /// <summary>
  /// Met cet emplacement sous le controle de ce type d'agent, ou de l'humain quand le
  /// type est vide ou vaut "HUMAN".
  ///
  /// Rend faux si le type est inconnu ou si l'emplacement ne peut pas etre bascule -
  /// l'appelant laisse alors les choses en l'etat plutot que de tomber.
  /// </summary>
  bool SetPlayerType(int playerIndex, string type);

  /// <summary>Le type qui pilote cet emplacement, ou "HUMAN".</summary>
  string GetPlayerType(int playerIndex);
}
