# Where Ada.Url and System.Uri disagree

Generated from the vendored web-platform-tests URL corpus. Do not edit by hand.

`System.Uri` is not wrong here, and neither is Ada. They implement different
specifications. Ada follows the WHATWG URL Standard, which is what browsers, Node, Go
and Python implement. `System.Uri` follows RFC 3986 and 3987 plus a decade of .NET
specific behaviour. This file exists so the difference is a decision you make rather
than a surprise you discover in production.

Only absolute URLs are compared. Relative references need a base, and `System.Uri`
resolves those through a different constructor with different rules, so comparing
them would measure the comparison rather than the parsers.

## Summary

| | Count |
| --- | ---: |
| Absolute cases compared | 538 |
| Same result | 352 |
| Accepted by Ada, rejected by System.Uri | 64 |
| Rejected by Ada, accepted by System.Uri | 32 |
| Both parsed, different serialisation | 90 |

**Rejected by Ada, accepted by System.Uri** is the row that matters for security, and it
holds 32 of the 538 cases. Each one is an input `System.Uri` accepts that
browsers, Node, Go and Python all refuse. Code that validates a URL with one parser and
then fetches it with another has an exploitable gap exactly there.

The other direction, 64 inputs Ada accepts and `System.Uri` rejects, is a
compatibility question rather than a security one. Those are URLs the rest of the web
handles and .NET currently does not.

## Different serialisation

| Input | Ada.Url | System.Uri |
| --- | --- | --- |
| <code>about:/../</code> | <code>about:/</code> | <code>about:/../</code> |
| <code>about:blank#\u0000\u0001\t\n\r\u001f !"#$%&amp;'()*+,-./09:;&lt;=&gt;?@AZ[\]^_&#96;az{&#124;}~\u007f\u0080\u0081Éé</code> | <code>about:blank#%00%01%1F%20!%22#$%&amp;'()*+,-./09:;%3C=%3E?@AZ[\]^_%60az{&#124;}~%7F%C2%80%C2%81%C3%89%C3%A9</code> | <code>about:blank#%00%01%09%0A%0D%1F%20!%22#$%25&amp;'()*+,-./09:;%3C=%3E?@AZ[%5C]%5E_%60az%7B%7C%7D~%7F%C2%80%C2%81%C3%89%C3%A9</code> |
| <code>android-app://x:0</code> | <code>android-app://x:0</code> | <code>android-app://x:0/</code> |
| <code>chrome-distiller://x:0</code> | <code>chrome-distiller://x:0</code> | <code>chrome-distiller://x:0/</code> |
| <code>chrome-extension://x:0</code> | <code>chrome-extension://x:0</code> | <code>chrome-extension://x:0/</code> |
| <code>chrome-native://x:0</code> | <code>chrome-native://x:0</code> | <code>chrome-native://x:0/</code> |
| <code>chrome-resource://x:0</code> | <code>chrome-resource://x:0</code> | <code>chrome-resource://x:0/</code> |
| <code>chrome-search://x:0</code> | <code>chrome-search://x:0</code> | <code>chrome-search://x:0/</code> |
| <code>data:/../</code> | <code>data:/</code> | <code>data:/../</code> |
| <code>data:text/plain,test#\u0000\u0001\t\n\r\u001f !"#$%&amp;'()*+,-./09:;&lt;=&gt;?@AZ[\]^_&#96;az{&#124;}~\u007f\u0080\u0081Éé</code> | <code>data:text/plain,test#%00%01%1F%20!%22#$%&amp;'()*+,-./09:;%3C=%3E?@AZ[\]^_%60az{&#124;}~%7F%C2%80%C2%81%C3%89%C3%A9</code> | <code>data:text/plain,test#%00%01%09%0A%0D%1F%20!%22#$%25&amp;'()*+,-./09:;%3C=%3E?@AZ[%5C]%5E_%60az%7B%7C%7D~%7F%C2%80%C2%81%C3%89%C3%A9</code> |
| <code>file:////foo</code> | <code>file:////foo</code> | <code>file://foo/</code> |
| <code>file://\/localhost//cat</code> | <code>file:////localhost//cat</code> | <code>file://localhost//cat</code> |
| <code>file://a­b/p</code> | <code>file://ab/p</code> | <code>file://a­b/p</code> |
| <code>file://loC𝐀𝐋𝐇𝐨𝐬𝐭/usr/bin</code> | <code>file:///usr/bin</code> | <code>file://loc𝐀𝐋𝐇𝐨𝐬𝐭/usr/bin</code> |
| <code>file://localhost////foo</code> | <code>file://////foo</code> | <code>file://localhost////foo</code> |
| <code>file://localhost//a//../..//</code> | <code>file://///</code> | <code>file://localhost///</code> |
| <code>file://localhost//a//../..//foo</code> | <code>file://///foo</code> | <code>file://localhost///foo</code> |
| <code>file:\\localhost//</code> | <code>file:////</code> | <code>file://localhost//</code> |
| <code>foo://host/ !"$%&amp;'()*+,-./:;&lt;=&gt;@[\]^_&#96;{&#124;}~</code> | <code>foo://host/%20!%22$%&amp;'()*+,-./:;%3C=%3E@[\]%5E_%60%7B&#124;%7D~</code> | <code>foo://host/%20!%22$%25&amp;'()*+,-./:;%3C=%3E@[/]%5E_%60%7B%7C%7D~</code> |
| <code>foo://host/dir/# !"#$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_&#96;{&#124;}~</code> | <code>foo://host/dir/#%20!%22#$%&amp;'()*+,-./:;%3C=%3E?@[\]^_%60{&#124;}~</code> | <code>foo://host/dir/#%20!%22#$%25&amp;'()*+,-./:;%3C=%3E?@[%5C]%5E_%60%7B%7C%7D~</code> |
| <code>foo://host/dir/? !"$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_&#96;{&#124;}~</code> | <code>foo://host/dir/?%20!%22$%&amp;'()*+,-./:;%3C=%3E?@[\]^_&#96;{&#124;}~</code> | <code>foo://host/dir/?%20!%22$%25&amp;'()*+,-./:;%3C=%3E?@[%5C]%5E_%60%7B%7C%7D~</code> |
| <code>fuchsia-dir://x:0</code> | <code>fuchsia-dir://x:0</code> | <code>fuchsia-dir://x:0/</code> |
| <code>gopher://foo:70/</code> | <code>gopher://foo:70/</code> | <code>gopher://foo/</code> |
| <code>h://.</code> | <code>h://.</code> | <code>file:///h://</code> |
| <code>http://:@www.example.com</code> | <code>http://www.example.com/</code> | <code>http://:@www.example.com/</code> |
| <code>http://@pple.com</code> | <code>http://pple.com/</code> | <code>http://@pple.com/</code> |
| <code>http://@www.example.com</code> | <code>http://www.example.com/</code> | <code>http://@www.example.com/</code> |
| <code>http://a:@www.example.com</code> | <code>http://a@www.example.com/</code> | <code>http://a:@www.example.com/</code> |
| <code>http://example.com/foo\tbar</code> | <code>http://example.com/foobar</code> | <code>http://example.com/foo%09bar</code> |
| <code>http://example.com/foo\t\u0091%91</code> | <code>http://example.com/foo%C2%91%91</code> | <code>http://example.com/foo%09%C2%91%91</code> |
| <code>http://example.com/foo%</code> | <code>http://example.com/foo%</code> | <code>http://example.com/foo%25</code> |
| <code>http://example.com/foo%00%51</code> | <code>http://example.com/foo%00%51</code> | <code>http://example.com/foo%00Q</code> |
| <code>http://example.com/foo%2</code> | <code>http://example.com/foo%2</code> | <code>http://example.com/foo%252</code> |
| <code>http://example.com/foo%2zbar</code> | <code>http://example.com/foo%2zbar</code> | <code>http://example.com/foo%252zbar</code> |
| <code>http://example.com/foo%2Â©zbar</code> | <code>http://example.com/foo%2%C3%82%C2%A9zbar</code> | <code>http://example.com/foo%252%C3%82%C2%A9zbar</code> |
| <code>http://example.com/foo%41%7a</code> | <code>http://example.com/foo%41%7a</code> | <code>http://example.com/fooAz</code> |
| <code>http://example.com/foo/%2e%2</code> | <code>http://example.com/foo/%2e%2</code> | <code>http://example.com/foo/.%252</code> |
| <code>http://example.com/foo/%2e./%2e%2e/.%2e/%2e.bar</code> | <code>http://example.com/%2e.bar</code> | <code>http://example.com/..bar</code> |
| <code>http://example.org/test?%GH</code> | <code>http://example.org/test?%GH</code> | <code>http://example.org/test?%25GH</code> |
| <code>http://example.org/test?a#%GH</code> | <code>http://example.org/test?a#%GH</code> | <code>http://example.org/test?a#%25GH</code> |
| <code>http://host/?'</code> | <code>http://host/?%27</code> | <code>http://host/?'</code> |
| <code>http://www/foo%2Ehtml</code> | <code>http://www/foo%2Ehtml</code> | <code>http://www/foo.html</code> |
| <code>http://é@é</code> | <code>http://%C3%A9@xn--9ca/</code> | <code>http://%C3%A9@é/</code> |
| <code>https://0x.0x.0</code> | <code>https://0.0.0.0/</code> | <code>https://0x.0x.0/</code> |
| <code>https://0x.0x.0x.0x</code> | <code>https://0.0.0.0/</code> | <code>https://0x.0x.0x.0x/</code> |
| <code>https://:@test</code> | <code>https://test/</code> | <code>https://:@test/</code> |
| <code>https://faß.ExAmPlE/</code> | <code>https://xn--fa-hia.example/</code> | <code>https://faß.example/</code> |
| <code>https://test:@test</code> | <code>https://test@test/</code> | <code>https://test:@test/</code> |
| <code>https://www.example.com/path{\u007fpath.html?query'\u007f=query#fragment&lt;\u007ffragment</code> | <code>https://www.example.com/path%7B%7Fpath.html?query%27%7F=query#fragment%3C%7Ffragment</code> | <code>https://www.example.com/path%7B%7Fpath.html?query'%7F=query#fragment%3C%7Ffragment</code> |
| <code>isolated-app://x:0</code> | <code>isolated-app://x:0</code> | <code>isolated-app://x:0/</code> |
| <code>javascript:/../</code> | <code>javascript:/</code> | <code>javascript:/../</code> |
| <code>ldap://localhost:389/ou=People,o=JNDITutorial</code> | <code>ldap://localhost:389/ou=People,o=JNDITutorial</code> | <code>ldap://localhost/ou=People,o=JNDITutorial</code> |
| <code>lolscheme:x x#x x</code> | <code>lolscheme:x x#x%20x</code> | <code>lolscheme:x%20x#x%20x</code> |
| <code>mailto:/../</code> | <code>mailto:/</code> | <code>mailto:/../</code> |
| <code>mailto://test/a/../b</code> | <code>mailto://test/b</code> | <code>mailto://test/a/../b</code> |
| <code>non-spec:/..//</code> | <code>non-spec:/.//</code> | <code>non-spec:/..//</code> |
| <code>non-spec:/..//path</code> | <code>non-spec:/.//path</code> | <code>non-spec:/..//path</code> |
| <code>non-spec:/a/..//</code> | <code>non-spec:/.//</code> | <code>non-spec:/a/..//</code> |
| <code>non-spec:/a/..//path</code> | <code>non-spec:/.//path</code> | <code>non-spec:/a/..//path</code> |
| <code>non-special://:@test/x</code> | <code>non-special://test/x</code> | <code>non-special://:@test/x</code> |
| <code>non-special://host/a\b</code> | <code>non-special://host/a\b</code> | <code>non-special://host/a/b</code> |
| <code>non-special://test:@test/x</code> | <code>non-special://test@test/x</code> | <code>non-special://test:@test/x</code> |
| <code>non-special:/\path</code> | <code>non-special:/\path</code> | <code>non-special://path/</code> |
| <code>non-special:\/opaque</code> | <code>non-special:\/opaque</code> | <code>non-special://opaque/</code> |
| <code>non-special:\\opaque</code> | <code>non-special:\\opaque</code> | <code>non-special://opaque/</code> |
| <code>non-special:\\opaque/path</code> | <code>non-special:\\opaque/path</code> | <code>non-special://opaque/path</code> |
| <code>non-special:cannot-be-a-base-url-!"$%&amp;'()*+,-.;&lt;=&gt;@[\]^_&#96;{&#124;}~@/</code> | <code>non-special:cannot-be-a-base-url-!"$%&amp;'()*+,-.;&lt;=&gt;@[\]^_&#96;{&#124;}~@/</code> | <code>non-special:cannot-be-a-base-url-!%22$%25&amp;'()*+,-.;%3C=%3E@[%5C]%5E_%60%7B%7C%7D~@/</code> |
| <code>non-special:opaque\t\t  \r #hi</code> | <code>non-special:opaque  %20#hi</code> | <code>non-special:opaque%09%09%20%20%0D%20#hi</code> |
| <code>non-special:opaque \t\t  \t#hi</code> | <code>non-special:opaque  %20#hi</code> | <code>non-special:opaque%20%09%09%20%20%09#hi</code> |
| <code>non-special:opaque \t\t  #hi</code> | <code>non-special:opaque  %20#hi</code> | <code>non-special:opaque%20%09%09%20%20#hi</code> |
| <code>non-special:opaque  #hi</code> | <code>non-special:opaque %20#hi</code> | <code>non-special:opaque%20%20#hi</code> |
| <code>non-special:opaque  ?hi</code> | <code>non-special:opaque %20?hi</code> | <code>non-special:opaque%20%20?hi</code> |
| <code>non-special:opaque  x#hi</code> | <code>non-special:opaque  x#hi</code> | <code>non-special:opaque%20%20x#hi</code> |
| <code>non-special:opaque  x?hi</code> | <code>non-special:opaque  x?hi</code> | <code>non-special:opaque%20%20x?hi</code> |
| <code>sc://#</code> | <code>sc://#</code> | <code>sc:///#</code> |
| <code>sc://?</code> | <code>sc://?</code> | <code>sc:///?</code> |
| <code>sc://faß.ExAmPlE/</code> | <code>sc://fa%C3%9F.ExAmPlE/</code> | <code>sc://faß.example/</code> |
| <code>sc://ñ</code> | <code>sc://%C3%B1</code> | <code>sc://ñ/</code> |
| <code>sc://ñ#x</code> | <code>sc://%C3%B1#x</code> | <code>sc://ñ/#x</code> |
| <code>sc://ñ.test/</code> | <code>sc://%C3%B1.test/</code> | <code>sc://ñ.test/</code> |
| <code>sc://ñ?x</code> | <code>sc://%C3%B1?x</code> | <code>sc://ñ/?x</code> |
| <code>sc:\../</code> | <code>sc:\../</code> | <code>sc:%5C../</code> |
| <code>telnet://user:pass@foobar.com:23/</code> | <code>telnet://user:pass@foobar.com:23/</code> | <code>telnet://user:pass@foobar.com/</code> |
| <code>w://x:0</code> | <code>w://x:0</code> | <code>file:///w://x:0</code> |
| <code>west://x:0</code> | <code>west://x:0</code> | <code>west://x:0/</code> |
| <code>wow:%1G</code> | <code>wow:%1G</code> | <code>wow:%251G</code> |
| <code>wow:%NBD</code> | <code>wow:%NBD</code> | <code>wow:%25NBD</code> |
| <code>wss://host/ !"$%&amp;'()*+,-./:;&lt;=&gt;@[\]^_&#96;{&#124;}~</code> | <code>wss://host/%20!%22$%&amp;'()*+,-./:;%3C=%3E@[/]%5E_%60%7B&#124;%7D~</code> | <code>wss://host/%20!%22$%25&amp;'()*+,-./:;%3C=%3E@[/]%5E_%60%7B%7C%7D~</code> |
| <code>wss://host/dir/# !"#$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_&#96;{&#124;}~</code> | <code>wss://host/dir/#%20!%22#$%&amp;'()*+,-./:;%3C=%3E?@[\]^_%60{&#124;}~</code> | <code>wss://host/dir/#%20!%22#$%25&amp;'()*+,-./:;%3C=%3E?@[%5C]%5E_%60%7B%7C%7D~</code> |
| <code>wss://host/dir/? !"$%&amp;'()*+,-./:;&lt;=&gt;?@[\]^_&#96;{&#124;}~</code> | <code>wss://host/dir/?%20!%22$%&amp;%27()*+,-./:;%3C=%3E?@[\]^_&#96;{&#124;}~</code> | <code>wss://host/dir/?%20!%22$%25&amp;'()*+,-./:;%3C=%3E?@[%5C]%5E_%60%7B%7C%7D~</code> |
