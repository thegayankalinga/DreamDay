/*!
    * Start Bootstrap - SB Admin v7.0.7 (https://startbootstrap.com/template/sb-admin)
    * Copyright 2013-2023 Start Bootstrap
    * Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-sb-admin/blob/master/LICENSE)
    */
    // 
// Scripts
// 

window.addEventListener('DOMContentLoaded', event => {
    const sidebarToggle = document.body.querySelector('#sidebarToggle');
    const sidenav = document.body.querySelector('#layoutSidenav_nav');

    if (sidebarToggle && sidenav) {
        // Restore previous state
        if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
            document.body.classList.toggle('sb-sidenav-toggled');
            sidenav.classList.add('sb-sidenav-collapsed');
        }

        // Toggle on click
        sidebarToggle.addEventListener('click', e => {
            e.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            sidenav.classList.toggle('sb-sidenav-collapsed');

            const isCollapsed = document.body.classList.contains('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', isCollapsed);
        });
    }
});

