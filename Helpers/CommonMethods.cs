namespace VityazReports.Helpers {
    public class CommonMethods {
        public int ParseDigit(string param) {
            string r = null;
            if (string.IsNullOrEmpty(param))
                return 0;
            else if (!int.TryParse(param, out _)) {
                char[] arr = param.ToCharArray();
                foreach (var item in arr) {
                    if (char.IsDigit(item)) {
                        r += item;
                    }
                    else if (char.IsPunctuation(item)) {
                        if (!string.IsNullOrEmpty(r))
                            return int.Parse(r);
                        else
                            return 0;
                    }
                }
                if (string.IsNullOrEmpty(r))
                    return 0;
                return int.Parse(r);
            }
            else
                return int.Parse(param);
        }
    }
}
