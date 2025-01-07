//Copy cả controller và view

$(document).ready(function () {


    function formatCurrency(amount) {
        const formatter = new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        });

        return formatter.format(amount);
    }
    var priceValue = $("#priceInput").val();
    $("#formattedPrice").text(formatCurrency(priceValue));
    $("#priceInput").on("input", function () {
        const price = $(this).val();
        if (price) {
            $("#formattedPrice").text(formatCurrency(price));
        } else {
            $("#formattedPrice").text("");
        }
    });
    $("#formEdit").on("submit", function (e) {
        e.preventDefault();
        var duration = $("#durationSelect").val()
        var namePrice = $("#NamePrice").val()
        var price = $("#priceInput").val()

        if (!namePrice || price <= 0 || !duration) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: `Please fill in all fields correctly.`,
                confirmButtonText: 'OK'
            }).then((result) => {
                if (result.isConfirmed) {
                    return;

                }
            });

            return;
        }
        // Tạo form data
        let formData = new FormData();
        formData.append("IDMemberShipPrice", $("#IDMemberShipPrice").val());
        formData.append("Duration", duration);
        formData.append("NamePrice", namePrice);
        formData.append("Price", price);
        axios
            .post("/system/edit-price", formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            })
            .then(function (response) {
                const res = response.data;
                if (res.code === 200) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Add successfully',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });

                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: `${response.data.message}`,
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            return;

                        }
                    });
                }
            })
            .catch(function (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error has occurred',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        return;

                    }
                });
            });


    });
});