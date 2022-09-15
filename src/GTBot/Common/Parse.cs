namespace GTBot.Common
{
    public class data
    {
        public string key;
        public string value;
    }

    public class Parse
    {
        private readonly List<data> DATA = new List<data>();

        public Parse(string str = "") => unpack(str);

        //Yes really shit but it works..
        public void unpack(string str)
        {
            str = str.Replace("Server is under maintenance. We will be back online shortly. Thank you for your patience!", "").Replace(" ", "\n");

            var lines = str.Split("\n");

            foreach(var line in lines)
            {
                var key = string.Empty;
                var value = string.Empty;
                var ch = line.ToCharArray();
                var isKey = true;
                foreach (var ch2 in ch)
                    if (ch2 != '|' && isKey) key += ch2;
                    else if (ch2 == '|') isKey = false;
                    else if (ch2 != '|' && !isKey) value += ch2;
                DATA.Add(new data() { key = key, value = value });
            }
        }

        public void set(string key, string value)
        {
            for (var i = 0; i < DATA.Count; i++)
                if (DATA[i].key == key)
                    DATA[i].value = value;
        }

        public string get(string key)
        {
            var rtnstrng = string.Empty;
            for (var i = 0; i < DATA.Count; i++)
                if (DATA[i].key == key)
                {
                    rtnstrng = DATA[i].value;
                    break;
                }

            return rtnstrng;
        }

        public void delete(string key)
        {
            for (var i = 0; i < DATA.Count; i++)
                if (DATA[i].key == key)
                    DATA.RemoveAt(i);
        }

        public void append(string key, string value)
        {
            DATA.Add(new data() { key = key, value = value });
        }

        public string raw()
        {
            var raw_data = string.Empty;
            for (var i = 0; i < DATA.Count; i++) raw_data += DATA[i].key + "|" + DATA[i].value + "\n";
            return raw_data;
        }
    }
}
