/** layuiAdmin.pro-v1.2.1 LPPL License By http://www.layui.com/admin/ */
; function clickHandler(e, n, t) { for (var c = 0, i = e.length; c < i; c++)e[c].className = ""; t.className = "focus"; for (var l = t.getAttribute("tabSrc"), r = 0, a = n.length; r < a; r++) { var s = n[r], o = s.getAttribute("id"); s.onclick = function () { this.style.zoom = 1 }, o != l ? s.style.zIndex = 1 : s.style.zIndex = 200 } } function switchTab(e) { for (var n = $G(e).children, t = n[0].children, c = n[1].children, i = 0, l = t.length; i < l; i++) { var r = t[i]; "focus" === r.className && clickHandler(t, c, r), r.onclick = function () { clickHandler(t, c, this) } } } switchTab("helptab"), document.getElementById("version").innerHTML = parent.UE.version;