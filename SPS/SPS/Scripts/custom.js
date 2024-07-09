(function ($) {
    "use strict"; // Start of use strict

})(jQuery); // End of use strict


// Event to remove violation when clicking image
jQuery.event.special.touchstart = {
    setup: function (_, ns, handle) {
        if (ns.includes("noPreventDefault")) {
            this.addEventListener("touchstart", handle, { passive: false });
        } else {
            this.addEventListener("touchstart", handle, { passive: true });
        }
    }
};
jQuery.event.special.touchmove = {
    setup: function (_, ns, handle) {
        if (ns.includes("noPreventDefault")) {
            this.addEventListener("touchmove", handle, { passive: false });
        } else {
            this.addEventListener("touchmove", handle, { passive: true });
        }
    }
};