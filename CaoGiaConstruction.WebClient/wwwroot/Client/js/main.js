(function ($) {
    "use strict";

    $(window).load(function () {
        loading.hide();
        $('body').find('.section').each(function () {
            let $section = $(this);
            new Waypoint({
                element: $section[0],
                handler: function (direction) {
                    let $element = $(this.element);
                    $element.find('.animated').each(function () {
                        let $animate = $(this).attr('data-animate');
                        if (typeof $animate !== "undefined") {
                            $(this).addClass($animate);
                        };
                    });
                },
                offset: '70%',
            });
        });
    });

    $(document).ready(function () {
        loading.show();
        // Hover Menu
        $('.navbar-nav > li .mega-menu').addClass('animated animation-duration-5');
        $('.navbar-nav > li .dropdown').addClass('animated animation-duration-5');

        $(".navbar-nav > li").hover(function () {
            $(this).children().addClass('fadeInUpMenu');
        }, function () {
            $(this).children().removeClass('fadeInUpMenu');
        });

        // Nivo Slider;
        const $slider = $('#banner .slider-wrapper');
        if ($slider.length > 0) {
            $slider.imagesLoaded(function () {
                $slider.find('.nivoSlider').nivoSlider({
                    pauseTime: 10000,
                    beforeChange: function () {
                    },
                    afterChange: function () {
                        $slider.find('.nivo-caption').find('.animated').each(function () {
                            $(this).addClass($(this).attr('data-animate'));
                        });
                    },
                    slideshowEnd: function () {
                    },
                    lastSlide: function () {
                    },
                    afterLoad: function () {
                        $slider.find('.nivo-caption').find('.animated').each(function () {
                            $(this).addClass($(this).attr('data-animate'));
                        });
                    },
                });
            });
        }// End Nivo

        // Services
        let $check_click = false;
        $('.section.services').on('click', 'li.service:not(.active)', function (e) {
            e.preventDefault();

            let $this = $(this);

            if (!$this.hasClass('active')) {
                $('.section.services').find('li.service').removeClass('active');
                $this.addClass('active');

                $('.section.services').find('li.service .animated').addClass('bullets');
                $this.find('.animated').removeClass('bullets');
            }

            $check_click = true;
        });

        $('.section.services').on('click', 'li.service.active', function (e) {
            $check_click = true;
        });

        $('.section.services').on('click', function (e) {
            if ($check_click == false) {
                e.preventDefault();
                $('#services-item').find('li.service').removeClass('active');
            }
            $check_click = false;
        });

        // Load Menus
        let $menus = $('#menus');
        if ($menus.length > 0) {
            // filter items
            $menus.find('.filter').on('click', 'button', function () {
                let $this = $(this),
                    filterValue = $this.attr('data-filter');

                $this.addClass('active');
                $this.siblings('.active').removeClass('active');

                if (filterValue == '*') filterValue = '.menu-item';

                $menus.find('.menu-item').not(filterValue).fadeOut(1000);
                $menus.find(filterValue).fadeIn(1000);

                setTimeout(function () { Waypoint.refreshAll(); }, 1500);

                return false;
            });
        }

        // Flickity slider
        if ($('.gallery-about').length > 0) {
            $('.gallery-about .gallery-flickity').imagesLoaded(function () {
                $('.gallery-about .gallery-flickity').flickity({
                    freeScroll: true,
                    contain: true,
                    // disable previous & next buttons and dots
                    prevNextButtons: false,
                    pageDots: false,
                });
            });
        }

        function gallery_flickity($gallery) {
            if ($gallery.length > 0) {
                $gallery.imagesLoaded(function () {
                    $gallery.flickity({
                        // options
                        wrapAround: true,
                        imagesLoaded: true,
                        resize: false,
                        arrowShape: {
                            x0: 25,
                            x1: 60, y1: 35,
                            x2: 70, y2: 35,
                            x3: 35
                        }
                    });
                });
            }
        }

        let $galleries = $('#galleries');
        if ($galleries.length > 0) {
            // filter items
            $galleries.find('.filter').on('click', 'button', function () {
                let $this = $(this),
                    filterValue = $this.attr('data-filter'),
                    $item = $galleries.find('[class^="col"].active');

                $this.addClass('active');
                $this.siblings('.active').removeClass('active');

                $item.css('height', $item.find('.inner')
                    .innerHeight())
                    .removeClass('active');

                if (filterValue == '*') filterValue = '.gallery-item';

                $galleries.find('.gallery-item').not(filterValue).fadeOut(1000);
                $galleries.find(filterValue).fadeIn(1000);

                return false;
            });

            // Close gallery
            $galleries.on('click', '.gallery-v1 .close', function (e) {
                e.preventDefault();

                let $this = $(this),
                    $parent = $this.parents('.gallery-item');

                $parent.css('height', $parent.find('.inner')
                    .innerHeight())
                    .removeClass('active');
            });

            // Load detail
            $galleries.on('click', 'a.gallery-ajax', function (e) {
                e.preventDefault();

                let $this = $(this),
                    $parent = $this.parents('.gallery-item'),
                    $url = $this.attr('data-url');

                if ($this.parents('.galleries-v2').length > 0) {
                    let $modal = $parent.find('.modal');

                    if ($modal.length > 0)
                        $modal.addClass('animated');

                    if ($parent.find('.modal-body .gallery').length < 1) {
                        $(document).ajaxStart(function () {
                            //$('body').addClass('loadpage');
                        });
                        $(document).ajaxStop(function () {
                            //$('body').removeClass('loadpage');
                        });

                        $.ajax({
                            url: $url,
                            cache: false,
                        }).done(function (html) {
                            let $element = $(html).find('.gallery');

                            $parent.find('.modal-body').html($element);

                            // This Share
                            //stButtons.locateElements();

                            gallery_flickity($parent.find('.gallery-flickity'));
                        })

                            .fail(function () {
                                location.reload();
                            })
                            .always(function (html) {
                                let $gallery = $modal.find('.gallery'),
                                    settle = 1;

                                $gallery.on('settle', function () {
                                    if (settle == 1) {
                                        $('body').removeClass('loadpage');
                                        $modal.addClass('fadeInDown');
                                    }
                                    settle++;
                                });
                            });
                    } else {
                        $modal.addClass('fadeInDown');
                    }
                } else {
                    if ($parent.hasClass('active')) {
                        return false;
                    } else {
                        $parent.siblings('.active').css('height', $parent.find('.inner')
                            .innerHeight())
                            .removeClass('active');
                    }

                    if ($parent.find('.gallery').length < 1) {
                        $parent.css('height', $parent.find('.inner').innerHeight());

                        $(document).ajaxStart(function () {
                            // $('body').addClass('loadpage');
                        });
                        $(document).ajaxStop(function () {
                            //$('body').removeClass('loadpage');
                        });

                        $.ajax({
                            url: $url,
                            cache: false,
                        }).done(function (html) {
                            let $element = $(html).find('.gallery');

                            $parent.append($element);

                            //This Share
                            //stButtons.locateElements();

                            // Flickity slider
                            gallery_flickity($parent.find('.gallery-flickity'));
                        })

                            .fail(function () {
                                location.reload();
                            })
                            .always(function (html) {
                                let $gallery = $parent.find('.gallery'),
                                    settle = 1;

                                $gallery.on('settle', function () {
                                    if (settle == 1) {
                                        $('body').removeClass('loadpage');

                                        let $height = $gallery.innerHeight() + $parent.innerHeight();

                                        $parent.css("height", $height).addClass('active');
                                    }
                                    settle++;
                                });
                            });
                    } else {
                        let $gallery = $parent.find('.gallery'),
                            $height = $gallery.innerHeight() + $parent.innerHeight();

                        $parent.css("height", $height).addClass('active');
                    }
                }
            });
        }
    });

    // Filter Sticky
    let $filter = $('#filter'),
        $footer = $('#footer'),
        $check = false;
    if ($filter.length > 0) {
        new Waypoint.Sticky({
            element: $filter[0],
            wrapper: '<div class="filter-sticky" />',
            stuckClass: 'filter-stuck',
            offset: '0'
        });

        new Waypoint({
            element: $footer[0],
            handler: function (direction) {
                //console.log(direction);

                if (direction == 'down' && $filter.hasClass('filter-stuck')) {
                    $filter.removeClass('filter-stuck');
                    $check = true;
                } else if ($check && direction == 'up') {
                    $filter.addClass('filter-stuck');
                    $check = false;
                } else $check = false;
            },
            offset: '70%',
        });
    }

    // Style v2 Header Sticky
    let $header_inner = $('.style-v2 .header-inner, .style-v3 .header-inner');
    if ($header_inner.length > 0) {
        new Waypoint.Sticky({
            element: $header_inner[0],
            wrapper: '<div class="header-sticky" />',
            stuckClass: 'header-stuck',
            offset: '0'
        });
    }

})(jQuery);

const loading = {
    hide: (timeDelay = 350, timeOut = 350) => {
        const body = $('body');
        if (body.hasClass('loadpage')) {
            body.removeClass('loadpage');
        }
    },
    show: (timeOut = 350) => {
        $(".loadpage").fadeIn(timeOut);
    }
};
