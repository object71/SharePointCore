using System;

public static class Helpers
{
    public static void EnsureTrailingSlash(string url) {
        if(!string.IsNullOrWhiteSpace(url)) {
            if(url[url.Length - 1] != '/') {
                url += "/";
            }
        }
    }
}