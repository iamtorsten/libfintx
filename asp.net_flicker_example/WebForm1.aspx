<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="asp.net_flicker_example.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        //<![CDATA[

        var code, canvas, interval, inputbox;

        /* flicker code object */
        function flickerCode(newcode) {
            var code = newcode.toUpperCase().replace(/[^a-fA-F0-9]/g, '');

            this.getCode = function () {
                return code;
            }
        }

        /* flicker canvas */
        function flickerCanvas() {
            var code;
            var halfbyteid, clock, bitarray, canvas, ctx;

            this.reset = function () {
                halfbyteid = 0;
                clock = 1;
            }

            function setup() {
                var bits = new Object();
                /* bitfield: clock, bits 2^1, 2^2, 2^3, 2^4 */
                bits['0'] = [0, 0, 0, 0, 0];
                bits['1'] = [0, 1, 0, 0, 0];
                bits['2'] = [0, 0, 1, 0, 0];
                bits['3'] = [0, 1, 1, 0, 0];
                bits['4'] = [0, 0, 0, 1, 0];
                bits['5'] = [0, 1, 0, 1, 0];
                bits['6'] = [0, 0, 1, 1, 0];
                bits['7'] = [0, 1, 1, 1, 0];
                bits['8'] = [0, 0, 0, 0, 1];
                bits['9'] = [0, 1, 0, 0, 1];
                bits['A'] = [0, 0, 1, 0, 1];
                bits['B'] = [0, 1, 1, 0, 1];
                bits['C'] = [0, 0, 0, 1, 1];
                bits['D'] = [0, 1, 0, 1, 1];
                bits['E'] = [0, 0, 1, 1, 1];
                bits['F'] = [0, 1, 1, 1, 1];

                /* prepend synchronization identifier */
                code = '0FFF' + code;

                bitarray = new Array();
                for (i = 0; i < code.length; i += 2) {
                    bitarray[i] = bits[code[i + 1]]
                    bitarray[i + 1] = bits[code[i]]
                }
            }

            function createCanvas(width, height) {
                canvas = document.createElement('canvas');
                canvas.width = width;
                canvas.height = height;
                if (canvas.getContext) {
                    ctx = canvas.getContext('2d');
                }

                ctx.fillStyle = "rgb(0,0,0)";
                ctx.fillRect(0, 0, canvas.width, canvas.height);
            }

            this.step = function () {
                margin = 7;
                barwidth = canvas.width / 5;

                bitarray[halfbyteid][0] = clock;

                for (i = 0; i < 5; i++) {
                    if (bitarray[halfbyteid][i] == 1) {
                        ctx.fillStyle = "rgb(255,255,255)";
                    } else {
                        ctx.fillStyle = "rgb(0,0,0)";
                    }
                    ctx.fillRect(i * barwidth + margin, margin, barwidth - 2 * margin, canvas.height - 2 * margin);
                }

                clock--;
                if (clock < 0) {
                    clock = 1;

                    halfbyteid++;
                    if (halfbyteid >= bitarray.length) {
                        halfbyteid = 0;
                    }

                }

                return 0;
            }

            this.getCanvas = function () {
                return canvas;
            }

            this.setCode = function (newcode) {
                code = newcode.getCode();
                setup();
                this.reset();
            }

            createCanvas(205, 100);
        }

        function startFlicker() {
            interval = window.setInterval(step, 50);
        }

        function stopFlicker() {
            window.clearInterval(interval);
        }

        function setNewCode() {
            stopFlicker();

            code = new flickerCode("1784011041875F051234567890041203000044302C323015");
            canvas.setCode(code);

            startFlicker();
        }

        function step() {
            canvas.step();
        }

        function setup() {
            canvas = new flickerCanvas();
            document.getElementById('flickercontainer').appendChild(canvas.getCanvas());

            setNewCode();
        }

        window.addEventListener('load', setup, false);

//]]>
    </script>

    <style type="text/css">
        #code {
            width: 30em;
        }

        #flickercontainer {
            margin: 0 auto;
        }

        .msg {
            background-color: #fdd;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="flickercontainer"></div>
    </form>
</body>
</html>
