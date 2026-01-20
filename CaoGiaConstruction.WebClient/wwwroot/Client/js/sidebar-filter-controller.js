const productClientController = {
    data: {
        validateFormInstan: null,
        urlSearch: ""
    },
    init: function () {
        this.register();
    },
    register: function () {
        this.methods.initCheckboxAndSelectHandlers();
    },
    methods: {
        initCheckboxAndSelectHandlers: function () {
            this.initSelectBoxSort();
            this.initCheckBoxHandler("#form-category input[type='radio']", "category");
            this.initCheckBoxHandler("#form-price input[type='radio']", "price");
        },
        initSelectBoxSort: function () {
            $("#select-sort").on("change", function () {
                productClientController.methods.updateURL("sort");
            });
        },
        initCheckBoxHandler: function (selector, type) {
            $(selector).on("change", function () {
                productClientController.methods.updateURL(type);
            });
        },
        updateURL: function (changedType) {
            const categoryChecked = $("#form-category input[type='radio']:checked").val();
            const priceChecked = $("#form-price input[type='radio']:checked").val();
            const orderBy = $("#select-sort").val();
            const pathSegment = productClientController.methods.getPathSegment();
            let urlSearch = `${window.location.origin}/${pathSegment}`;
            let queryParams = [];

            if (categoryChecked) {
                urlSearch += `/${categoryChecked}`;
            }

            if (priceChecked) {
                queryParams.push(productClientController.methods.getPriceQueryString(priceChecked));
            }

            if (orderBy) {
                queryParams.push(`orderBy=${orderBy}`);
            }

            if (queryParams.length > 0) {
                urlSearch += `?${queryParams.join('&')}`;
            }
            window.location.href = urlSearch;
        },
        getPriceQueryString: function (price) {
            let queryString = "";
            switch (price) {
                case "100000":
                    queryString = "priceTo=100000";
                    break;
                case "100000-200000":
                    queryString = "priceFrom=100000&priceTo=200000";
                    break;
                case "200000-300000":
                    queryString = "priceFrom=200000&priceTo=300000";
                    break;
                case "300000-500000":
                    queryString = "priceFrom=300000&priceTo=500000";
                    break;
                case "500000-1000000":
                    queryString = "priceFrom=500000&priceTo=1000000";
                    break;
                case "1000000":
                    queryString = "priceFrom=1000000";
                    break;
                default:
                    queryString = "priceFrom=1000000";
                    break;
            }
            return queryString;
        },

        getPathSegment: function () {
            // Lấy path từ window.location.pathname
            const path = window.location.pathname;

            // Tách path thành các phần tử riêng biệt
            const segments = path.split('/').filter(Boolean);

            // Lấy phần tử đầu tiên, đây là phần path bạn muốn
            const firstSegment = segments[0];

            return firstSegment;
        }
    }
}

$(function () {
    productClientController.init();
});
