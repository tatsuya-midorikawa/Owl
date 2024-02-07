using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owl.psrx;
internal class Html {
  private static readonly string css = """
    img.screenshot-thumb {
        border: 2px solid rgb(19,112,171);
        width: 756px;
        margin-top: 5px;
        cursor: pointer;
    }

    img.screenshot {
        border: 2px solid rgb(19,112,171);
        margin-top: 5px;
    }
    """;

  private static readonly string js = """
    function zoomToggle(num)
    {
      const img = document.getElementById("ss-" + num);

      if (img.className == "screenshot") {
        img.className = "screenshot-thumb";
      }
      else {
        img.className = "screenshot";
      }
    }
    """;

  private static readonly string head = """
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    """;

  public static string MakeBodyContent(int i, string title, string pname, DateTime time) {
    return $"""
      <div id="Step{i}" tabindex="{i}">
        <p><b>Step {i}: ({time:yyyy/MM/dd HH:mm:ss.fffff})</b>&emsp;{pname}&emsp;({(string.IsNullOrWhiteSpace(title) ? "N/A" : title)})</p>
        <a onclick="zoomToggle({i})"><img id="ss-{i}" class="screenshot-thumb" alt="手順 {i} のスクリーン ショット。" title="クリックしてスクリーンショットを拡大/縮小します。" src="img/step_{i}.jpg"></a>
        <hr aria-hidden="true" />
      </div>
      """;
  }
  public static string Build(StringBuilder body) {
    return $"""
      <html>
        <head>
          {head}
          <style>
          {css}
          </style>
        </head>
        <body>
          {body}
          <script>
            {js}
          </script>
        </body>
      </html>
      """;
  } 
}
