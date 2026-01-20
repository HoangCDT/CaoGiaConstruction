$(document).ready(function () {

    // Khởi tạo
    initializeScrollToTop();
    handleScrollEvent();
    handleScrollToTopClick();

    // Common options for all carousels
    const commonOptions = {
        loop: true,
        autoplay: true,
        navSpeed: 100,
        dotsSpeed: 100,
        smartSpeed: 1000,
        dragEndSpeed: 1,
        autoplayHoverPause: true,
        margin: 30,
        navText: ["<i class='fa fa-chevron-left'></i>", "<i class='fa fa-chevron-right'></i>"]
    };

    // Function to initialize Owl Carousel
    function initializeOwlCarousel(selector, options) {
        $(selector).owlCarousel({
            ...commonOptions,
            ...options
        });
    }

    // Initialize carousels with specific options
    initializeOwlCarousel(".machine-wrapper", {
        autoplayTimeout: 10000,
        autoplaySpeed: 10000,
        nav: true,
        dots: false,
        responsive: {
            0: {
                items: 1,
                autoplay: false,
                nav: false
            },
            480: {
                items: 1,
                autoplay: false,
                nav: false
            },
            678: {
                items: 3,
                autoplay: false,
                nav: true
            },
            960: {
                items: 3,
                nav: true
            },
            1160: {
                items: 3,
                nav: true
            }
        }
    });

    initializeOwlCarousel(".hot-blog-wrapper", {
        autoplayTimeout: 5000,
        autoplaySpeed: 1000,
        nav: false,
        dots: false,
        items: 1
    });

    initializeOwlCarousel(".related-product-wrapper", {
        autoplayTimeout: 5000,
        autoplaySpeed: 1000,
        nav: false,
        dots: false,
        responsive: {
            0: {
                items: 1,
                autoplay: false,
                nav: false
            },
            480: {
                items: 1,
                autoplay: false,
                nav: false
            },
            678: {
                items: 3,
                autoplay: false,
                nav: true
            },
            960: {
                items: 3,
                nav: true
            },
            1160: {
                items: 3,
                nav: true
            }
        }
    });

    initializeOwlCarousel(".related-blog-wrapper", {
        autoplayTimeout: 5000,
        autoplaySpeed: 1000,
        autoplay: true,
        nav: false,
        dots: false,
        loop: true,
        responsive: {
            0: {
                items: 1,
            },
            480: {
                items: 1,
            },
            678: {
                items: 2,
            },
            960: {
                items: 3,
            },
            1160: {
                items: 3,
            }
        }
    });

    initializeOwlCarousel(".video-wrapper", {
        autoplayTimeout: 5000,
        autoplaySpeed: 1000,
        autoplay: true,
        nav: false,
        dots: true,
        loop: true,
        responsive: {
            0: {
                items: 1,
                dots: false,
            },
            480: {
                items: 1,
                dots: false,
            },
            678: {
                items: 2,
                dots: true,
            },
            960: {
                items: 3,
                dots: true,
            },
            1160: {
                items: 3,
                dots: true,
            }
        }
    });

    initializeOwlCarousel(".feedback-owl", {
        autoplayTimeout: 5000,
        autoplaySpeed: 1000,
        autoplay: true,
        nav: false,
        dots: true,
        loop: true,
        responsive: {
            0: {
                items: 1,
                dots: false,
            },
            480: {
                items: 1,
                dots: false,
            },
            678: {
                items: 2,
            },
            960: {
                items: 2,
            },
            1160: {
                items: 2,
            }
        }
    });

    $('.categories-box .open-close').click(function () {
        toggleSubCategory($(this));
    });

    $('.navbar-nav .open-close').click(function () {
        toggleSubMenu($(this));
    });

    $(".item-branch").click(function (e) {
        const mapIFrame = $(this).data('map');
        $('#branchMapFrame').attr('src', mapIFrame);
        $('.item-branch').removeClass('active');
        $(this).addClass('active');
    })

    //#region active tab product detail
    $(".ul-tabs-pro-detail li").click(function () {
        const tabs = $(this).data("tabs");
        $(".content-tabs-pro-detail, .ul-tabs-pro-detail li").removeClass("active");
        $(this).addClass("active");
        $("." + tabs).addClass("active");
    });
    //#endregion active tab product detail

    $(".product-contact").click(function () {
        // Kích hoạt tab "Gửi thông tin liên hệ"
        $(".ul-tabs-pro-detail li").removeClass("active");
        $(".content-tabs-pro-detail").removeClass("active");

        // Chọn tab với data-tabs="commentfb-pro-detail"
        $(".ul-tabs-pro-detail li[data-tabs='commentfb-pro-detail']").addClass("active");
        $(".content-tabs-pro-detail.commentfb-pro-detail").addClass("active");
    });

    //#region view more
    $(".btn--view-more").on("click", function (e) {
        e.preventDefault();
        let seft = $(this);
        seft.parents(".tabs-pro-detail")
            .find(".content-pro-detail")
            .toggleClass("expanded");
        $(this).toggleClass("active");
        $("html, body").animate(
            { scrollTop: $(".tabs-pro-detail").offset().top },
            "fast"
        );
        return false;
    });
    //#region view more

    //#region init fancyapp
    Fancybox.bind('[data-fancybox="gallery"]', {
    });

    // customs fancyapp for content
    if ($(".fancybox-content-custom").length > 0) {
        $(".fancybox-content-custom img").each(function () {
            const src = $(this).attr("src")
            if (src.includes("http")) {
                $(this).wrap(`<a href="${src}" data-fancybox="gallery"></a>`);
            }
        })
    }
    //#endregion init fancyapp

    /*Open filter*/
    $('.open-filters').click(function (e) {
        e.stopPropagation();
        $(this).toggleClass('openf');
        $('.sidebar').toggleClass('openf');
        $('.cate-overlay3').toggleClass('open');
    });

    $('.cate-overlay3, .filter-item').click(function (e) {
        $('.cate-overlay3.open').removeClass('open');
        $('.sidebar.openf').toggleClass('openf');
        $('#open-filters.openf').toggleClass('openf');
    });

    // Submit Form Contact
    $("#contact_form").submit(function (event) {
        event.preventDefault(); // Ngăn chặn hành vi gửi form mặc định
        let $this = $(this);
        $.ajax({
            type: "POST",
            data: $this.serialize(),
            url: '/post-contact',
            cache: false,
        }).done(function (res) {
            showAlert(res?.success, res.message);
            if (res?.success) {
                $this.trigger("reset"); // Reset giá trị của form sau khi thành công
            }
        }).fail(function () {
            showAlert(false, 'Đã xảy ra lỗi với yêu cầu này. Chúng tôi đang cố gắng sửa lỗi sớm nhất có thể.');
        });

        function showAlert(isSuccess, message) {
            const alertClass = isSuccess ? 'alert-success' : 'alert-danger';
            const title = isSuccess ? 'Thành công' : 'Thất bại';
            const $alert = $this.find('.alert');
            if ($alert.length < 1) {
                $this.append('<div role="alert" class="alert ' + alertClass + '"><strong>' + title + '</strong> - <span>' + message + '</span></div>');
                $alert = $this.find('.alert');
            } else {
                $alert.removeClass('alert-success alert-danger').addClass(alertClass).html('<strong>' + title + '</strong> - <span>' + message + '</span>');
            }
            $alert.hide().toggle(350).delay(10000).toggle(350);
        }
        return false; // Ngăn chặn việc gửi form truyền thống
    });
});

// Hàm để tạo nút scrollToTop nếu chưa tồn tại
function initializeScrollToTop() {
    if (!$('.scrollToTop').length) {
        $("body").append('<div class="scrollToTop"><img src="~/Client/icons/top.png" alt="Quay về đầu trang"/></div>');
    }
}

// Hàm xử lý sự kiện cuộn trang
function handleScrollEvent() {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.scrollToTop').fadeIn();
        } else {
            $('.scrollToTop').fadeOut();
        }
    });
}

// Hàm xử lý sự kiện nhấp vào nút scrollToTop
function handleScrollToTopClick() {
    $('body').on("click", ".scrollToTop", function () {
        $('html, body').animate({ scrollTop: 0 }, 800);
        return false;
    });
}

function toggleSubCategory(element) {
    element.toggleClass("active");
    element.closest('li').find('>ul').slideToggle("fast");

    const icon = element.find('i');
    if (element.hasClass('active')) {
        icon.removeClass('fa-angle-down').addClass('fa-angle-up');
    } else {
        icon.removeClass('fa-angle-up').addClass('fa-angle-down');
    }
}

function toggleSubMenu(element) {
    element.toggleClass("active");
    element.closest('li').find('> .dropdown').slideToggle("fast");

    const icon = element.find('i');
    if (element.hasClass('active')) {
        icon.removeClass('fa-angle-down').addClass('fa-angle-up');
    } else {
        icon.removeClass('fa-angle-up').addClass('fa-angle-down');
    }
}

//#region Fancyapp video

// Fires whenever a player has finished loading
function onPlayerReady(event) {
    event.target.playVideo();
}

// Fires when the player's state changes.
function onPlayerStateChange(event) {
    // Go to the next video after the current one is finished playing
    if (event.data === 0) {
        $.fancybox.next();
    }
}

// The API will call this function when the page has finished downloading the JavaScript for the player API
function onYouTubePlayerAPIReady() {
    // Initialise the fancyBox after the DOM is loaded
    $(document).ready(function () {
        $(".fancybox")
            .attr('rel', 'gallery')
            .fancybox({
                beforeShow: function () {
                    // Find the iframe ID
                    var id = $.fancybox.inner.find('iframe').attr('id');

                    // Create video player object and add event listeners
                    var player = new YT.Player(id, {
                        events: {
                            'onReady': onPlayerReady,
                            'onStateChange': onPlayerStateChange
                        }
                    });
                }
            });
    });
}
//#endregion

//#region Image Error Handler - Default image fallback
// Global handler for image load errors - use default image if image fails to load
$(document).ready(function() {
    // Set default image path
    const defaultImagePath = '/Admin/assets/images/no_image.png';
    
    // Handle images that fail to load
    $(document).on('error', 'img', function() {
        const $img = $(this);
        const currentSrc = $img.attr('src');
        
        // Only replace if it's not already the default image to avoid infinite loop
        if (currentSrc && !currentSrc.includes('no_image.png') && !currentSrc.includes('no_avatar.png')) {
            $img.attr('src', defaultImagePath);
            // Remove onerror handler to prevent infinite loop
            $img.off('error');
        }
    });
    
    // Also handle images that are already loaded but failed
    $('img').on('error', function() {
        const $img = $(this);
        const currentSrc = $img.attr('src');
        
        if (currentSrc && !currentSrc.includes('no_image.png') && !currentSrc.includes('no_avatar.png')) {
            $img.attr('src', defaultImagePath);
            $img.off('error');
        }
    });
    
    // For images with data-src (lazy loading), also handle errors
    $(document).on('error', 'img[data-src]', function() {
        const $img = $(this);
        const currentSrc = $img.attr('src') || $img.attr('data-src');
        
        if (currentSrc && !currentSrc.includes('no_image.png') && !currentSrc.includes('no_avatar.png')) {
            $img.attr('src', defaultImagePath);
            $img.attr('data-src', defaultImagePath);
            $img.off('error');
        }
    });
});
//#endregion
