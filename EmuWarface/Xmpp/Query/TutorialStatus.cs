using EmuWarface.Core;
using EmuWarface.Xmpp;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmuWarface.Xmpp.Query
{
    public static class TutorialStatus
    {
        /*<tutorial_status id="678d8734-cc8a-4472-bb87-19bdb40107a8" step="tutorial_started" event="0" />
         */

        /// <summary>
        /// Identificadores dos tres tutoriais. Sao fixos no cliente: ele manda
        /// esse GUID para dizer de qual tutorial esta falando.
        /// </summary>
        private static readonly Dictionary<string, int> Tutoriais = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "678d8734-cc8a-4472-bb87-19bdb40107a8", 0 },  // tutorial_1
            { "688d8633-1c8a-4d72-bc87-19bdb40117aa", 1 },  // tutorial_2
            { "678a4754-1d2d-1f72-cc87-19bdb40107a8", 2 },  // tutorial_3
        };

        /// <summary>
        /// O cliente avisa aqui o andamento do tutorial. O que interessa e o
        /// fim: step vazio com event="2" significa concluido.
        ///
        /// Antes este metodo era um //TODO vazio e o servidor respondia
        /// tutorial_passed=1 no login, entao os tres tutoriais nasciam
        /// marcados como feitos e o jogador nunca recebia as recompensas
        /// (3000 WF$ e duas pecas de equipamento por tutorial).
        /// </summary>
        [Query(IqType.Get, "tutorial_status")]
        public static void TutorialStatusSerializer(Client client, Iq iq)
        {
            var resposta = Xml.Element("tutorial_status");

            var id      = iq.Query.GetAttribute("id");
            var step    = iq.Query.GetAttribute("step");
            var evento  = iq.Query.GetAttribute("event");

            // So o fim do tutorial premia; os outros passos sao andamento.
            bool concluiu = string.IsNullOrEmpty(step) && evento == "2";

            int tutorialId;
            if (concluiu && client.Profile != null && !string.IsNullOrEmpty(id)
                && Tutoriais.TryGetValue(id, out tutorialId))
            {
                // CompleteTutorial devolve false se ja tinha sido feito,
                // para nao premiar de novo quem repete o tutorial.
                if (client.Profile.CompleteTutorial(tutorialId))
                {
                    Log.Info("[Tutorial] Profile {0} completed tutorial_{1}", client.ProfileId, tutorialId + 1);

                    resposta.Child(client.Profile.ProgressionSerialize());
                }
            }

            iq.SetQuery(resposta);
            client.QueryResult(iq);
        }
    }
}
