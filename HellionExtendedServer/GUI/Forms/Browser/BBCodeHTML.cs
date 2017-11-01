/*
Original JavaScript version (at <a href="http://blogs.stonesteps.ca/showpost.aspx?pid=33" target="_blank">http://blogs.stonesteps.ca/showpost.aspx?pid=33</a>)
    Copyright (c) 2005-2007, Stone Steps Inc.
    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice,
          this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright 
          notice, this list of conditions and the following disclaimer in the 
          documentation and/or other materials provided with the distribution.
        * Neither the name of Stone Steps Inc. nor the names of its contributors 
          may be used to endorse or promote products derived from this software 
          without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS 
    IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
    THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
    PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS 
    BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
    SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
    INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
    THE POSSIBILITY OF SUCH DAMAGE.
*/

/*
C# port
    Copyright (c) 2009 Charlie Somerville

    All redistributions must honour the above BSD license and maintain this notice
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HellionExtendedServer.GUI.Forms.Browser
{
    class taginfo_t
    {
        public string bbtag { get; set; }
        public string etag { get; set; }

        public taginfo_t(string bbtag, string etag)
        {
            this.bbtag = bbtag;
            this.etag = etag;
        }
    }
    public class BBCode
    {
        public string Html { get; set; }

        Stack<taginfo_t> opentags;
        bool crlf2br = true;
        bool noparse = false;
        int urlstart = -1;

        // acceptable BBcode tags, optionally prefixed with a slash
        Regex tagname_re = new Regex(@"^/?(?:b|i|u|pre|code|colou?r|noparse|url|s|q|blockquote|small|big|img|center|left|right|hr)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // color names or hex color
        Regex color_re = new Regex(@"^(:?black|silver|gray|white|maroon|red|purple|fuchsia|green|lime|olive|yellow|navy|blue|teal|aqua|#(?:[0-9a-f]{3})?[0-9a-f]{3})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // numbers
        Regex number_re = new Regex(@"^[\.0-9]{1,8}$", RegexOptions.Compiled);

        // reserved, unreserved, escaped and alpha-numeric [RFC2396]
        Regex uri_re = new Regex(@"^[-;/\?:@&=\+\$,_\.!~\*'\(\)%0-9a-z]{1,512}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // main regular expression: CRLF, [tag=option], [tag] or [/tag]
        Regex postfmt_re = new Regex(@"/([\r\n])|(?:\[([a-z]{1,16})(?:=([^\x00-\x1F""'\(\)<>\[\]]{1,256}))?\])|(?:\[/([a-z]{1,16})\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // check if it's a valid BBCode tag
        bool isValidTag(string str)
        {
            if (str == null || str == "")
                return false;

            return tagname_re.Match(str).Success;
        }

        public string textToHtmlCB(string mstr, string m1, string m2, string m3, string m4, int offset, string tstr)
        {
            //
            // CR LF sequences
            //
            if (m1 != null && m1.Length != 0)
            {
                if (!crlf2br)
                    return mstr;

                switch (m1)
                {
                    case "\r":
                        return "";
                    case "\n":
                        return "<br>";
                }
            }

            //
            // handle start tags
            //
            if (isValidTag(m2))
            {
                // if in the noparse state, just echo the tag
                if (noparse)
                    return "[" + m2 + "]";

                // ignore any tags if there's an open option-less [url] tag
                if (opentags.Count != 0 && opentags.First().bbtag == "url" && urlstart >= 0)
                    return "[" + m2 + "]";

                switch (m2)
                {
                    case "code":
                        opentags.Push(new taginfo_t(m2, "</code></pre>"));
                        crlf2br = false;
                        return "<pre><code>";

                    case "pre":
                        opentags.Push(new taginfo_t(m2, "</pre>"));
                        crlf2br = false;
                        return "<pre>";

                    case "color":
                    case "colour":
                        if (m3 == null || !color_re.Match(m3).Success)
                            m3 = "inherit";
                        opentags.Push(new taginfo_t(m2, "</span>"));
                        return "<span style=\"color: " + m3 + "\">";

                    case "s":
                        opentags.Push(new taginfo_t(m2, "</span>"));
                        return "<span style=\"text-decoration: line-through\">";

                    case "noparse":
                        noparse = true;
                        return "";

                    case "url":
                        opentags.Push(new taginfo_t(m2, "</a>"));

                        // check if there's a valid option
                        if (m3 != null && uri_re.Match(m3).Success)
                        {
                            // if there is, output a complete start anchor tag
                            urlstart = -1;
                            return "<a href=\"" + m3 + "\">";
                        }

                        // otherwise, remember the URL offset 
                        urlstart = mstr.Length + offset;

                        // and treat the text following [url] as a URL
                        return "<a href=\"";

                    case "q":
                    case "blockquote":
                        opentags.Push(new taginfo_t(m2, "</" + m2 + ">"));
                        return m3 != null && m3.Length != 0 && uri_re.Match(m3).Success ? "<" + m2 + " cite=\"" + m3 + "\">" : "<" + m2 + ">";

                    case "img":
                        if (m3 == null && m3.Length == 0)
                            return "";

                        return m3.IndexOf("http:") == 0 ? "<img src='" + m3 + "' />" : "<img src='http://" + m3 + "' />";

                    case "left":
                    case "right":
                        opentags.Push(new taginfo_t(m2, "</div>"));
                        return "<div style='text-align:" + m2 + ";'>";

                    case "hr":
                        return "<hr />";

                    default:
                        // [samp], [b], [i] and [u] don't need special processing
                        opentags.Push(new taginfo_t(m2, "</" + m2 + ">"));
                        return "<" + m2 + ">";

                }
            }

            //
            // process end tags
            //
            if (isValidTag(m4))
            {
                if (noparse)
                {
                    // if it's the closing noparse tag, flip the noparse state
                    if (m4 == "noparse")
                    {
                        noparse = false;
                        return "";
                    }

                    // otherwise just output the original text
                    return "[/" + m4 + "]";
                }

                // highlight mismatched end tags
                if (opentags.Count == 0 || opentags.First().bbtag != m4)
                    return "<span style=\"color: red\">[/" + m4 + "]</span>";

                if (m4 == "url")
                {
                    // if there was no option, use the content of the [url] tag
                    if (urlstart > 0)
                        return "\">" + tstr.Substring(urlstart, offset - urlstart) + opentags.Pop().etag;

                    // otherwise just close the tag
                    return opentags.Pop().etag;
                }
                else if (m4 == "code" || m4 == "pre")
                    crlf2br = true;

                // other tags require no special processing, just output the end tag
                return opentags.Pop().etag;
            }

            return mstr;
        }

        public BBCode(string bb)
        {
            string result;
            string endtags = "";

            // convert CRLF to <br> by default
            crlf2br = true;

            // create a new array for open tags
            if (opentags == null || opentags.Count != 0)
                opentags = new Stack<taginfo_t>();

            // run the text through main regular expression matcher
            //result = bb.replace(postfmt_re, textToHtmlCB);
            result = postfmt_re.Replace(bb, x => textToHtmlCB(
                x.Groups[0].Value,
                x.Groups[1].Value,
                x.Groups[2].Value,
                x.Groups[3].Value,
                x.Groups[4].Value,
                x.Groups[0].Index,
                bb
                ));

            // reset noparse, if it was unbalanced
            if (noparse)
                noparse = false;

            // if there are any unbalanced tags, make sure to close them
            if (opentags.Count > 0)
            {
                endtags = "";

                // if there's an open [url] at the top, close it
                if (opentags.First().bbtag == "url")
                {
                    opentags.Pop();
                    endtags += "\">" + bb.Substring(urlstart, bb.Length - urlstart) + "</a>";
                }

                // close remaining open tags
                while (opentags.Count > 0)
                    endtags += opentags.Pop().etag;
            }

            Html = endtags.Length > 0 ? result + endtags : result;
        }
    }
}