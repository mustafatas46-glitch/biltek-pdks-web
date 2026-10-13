// Ana ekrandaki F2/F3/F4/F5/F8/F9/F10 ve Esc kısayolları. Tarayıcının kendi F tuşu davranışı engellenir.
window.pdksKisayol = {
    bagla: function (ref) {
        window.__pdksKisayolRef = ref;
        if (window.__pdksKisayolBagli) return;
        window.__pdksKisayolBagli = true;
        document.addEventListener('keydown', function (e) {
            if (!window.__pdksKisayolRef) return;
            if (['F2', 'F3', 'F4', 'F5', 'F8', 'F9', 'F10', 'Escape'].indexOf(e.key) >= 0) {
                e.preventDefault();
                window.__pdksKisayolRef.invokeMethodAsync('TusaBasildi', e.key);
            }
        });
    },
    birak: function () { window.__pdksKisayolRef = null; }
};
