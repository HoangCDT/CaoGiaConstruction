/**
 * Client-side controller for vcProject (home page projects section).
 * Handles AJAX pagination and client-side filter without full page reload.
 */
var projectHomeController = {
    getPartialUrl: function () {
        var wrapper = document.getElementById('project-home-ajax-wrapper');
        return wrapper ? (wrapper.getAttribute('data-partial-url') || '/trang-chu/projects-partial') : '/trang-chu/projects-partial';
    },

    loadProjects: function (page, serviceId) {
        var wrapper = document.getElementById('project-home-ajax-wrapper');
        if (!wrapper) return;
        var url = this.getPartialUrl() + '?page=' + encodeURIComponent(page);
        if (serviceId) {
            url += '&serviceId=' + encodeURIComponent(serviceId);
        }
        wrapper.classList.add('project-home-loading');
        fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (r) { return r.text(); })
            .then(function (html) {
                wrapper.innerHTML = html;
                wrapper.classList.remove('project-home-loading');
                if (typeof history !== 'undefined' && history.replaceState) {
                    var pathname = window.location.pathname || '/';
                    var search = '?page=' + page;
                    if (serviceId) search += '&projectServiceId=' + serviceId;
                    history.replaceState({ page: page, projectServiceId: serviceId || null }, '', pathname + search);
                }
            })
            .catch(function () {
                wrapper.classList.remove('project-home-loading');
            });
    },

    init: function () {
        var wrapper = document.getElementById('project-home-ajax-wrapper');
        if (!wrapper) return;

        document.addEventListener('click', function (e) {
            if (!wrapper.contains(e.target)) return;

            var paginationLink = e.target.closest('.project-home-pagination a.page-item[href]');
            if (paginationLink && !paginationLink.classList.contains('disabled') && paginationLink.getAttribute('href') !== '#') {
                e.preventDefault();
                var href = paginationLink.getAttribute('href');
                if (href) {
                    var match = href.match(/[?&]page=(\d+)/);
                    var page = match ? parseInt(match[1], 10) : 1;
                    if (page >= 1) {
                        projectHomeController.loadProjects(page);
                    }
                }
                return;
            }

            var filterBtn = e.target.closest('.project-home-filter-btn');
            if (filterBtn) {
                e.preventDefault();
                var filterButtons = wrapper.querySelectorAll('.project-home-filter-btn');
                var cards = wrapper.querySelectorAll('.project-home-card');
                filterButtons.forEach(function (btn) { btn.classList.remove('active'); });
                filterBtn.classList.add('active');
                var filterServiceId = filterBtn.getAttribute('data-filter-service-id') || '';
                cards.forEach(function (card) {
                    var cardServiceId = card.getAttribute('data-service-id') || '';
                    var show = !filterServiceId || filterServiceId === '' || cardServiceId === filterServiceId;
                    card.style.opacity = show ? '1' : '0';
                    card.style.transform = show ? 'scale(1)' : 'scale(0.95)';
                    if (!show) {
                        setTimeout(function () {
                            if (card.style.opacity === '0') card.style.display = 'none';
                        }, 300);
                    } else {
                        card.style.display = '';
                    }
                });
            }
        });
    }
};

document.addEventListener('DOMContentLoaded', function () {
    projectHomeController.init();
});
