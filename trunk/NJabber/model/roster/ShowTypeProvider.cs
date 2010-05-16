using System.Collections.Generic;

namespace NJabber.model.roster
{
    public class ShowTypeProvider
    {

        private const string None = "NONE";
        private const string Away = "away";
        private const string Dnd = "dnd";
        private const string Xa = "xa";
        private readonly Dictionary<string,string>  data= new Dictionary<string, string>();

        public static ShowTypeProvider Instance = new ShowTypeProvider();

        public ShowTypeProvider()
        {
            
            data.Add(None, Properties.Resources.NONE );
            data.Add(Away, Properties.Resources.away);
            data.Add(Dnd,Properties.Resources.dnd);
            data.Add(Xa,Properties.Resources.xa);

        }

        public string Get(string key)
        {
            if(data.ContainsKey(key)) return data[key];
            return null;
        }



    }
}
