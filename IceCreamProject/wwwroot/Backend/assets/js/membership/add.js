
//Copy cả controller và view

$(document).ready(function () {
    function formatCurrency(amount) {
        const formatter = new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        });

        return formatter.format(amount);
    }

    $("#priceInput").on("input", function () {
        const price = $(this).val();
        if (price) {
            $("#formattedPrice").text(formatCurrency(price));
        } else {
            $("#formattedPrice").text("");
        }
    });

    //add price
    $("#submitButton").on("click", function () {
        var duration = $("#durationSelect").val()
        var namePrice = $("#namePrice").val()
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
        const formData = new FormData();

        formData.append("Duration", duration);
        formData.append("NamePrice", namePrice);
        formData.append("Price", price);


        axios
            .post("/system/add-price", formData, {
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


    //Xóa price

    $(".remove-price").on("click", function () {
        const idPrice = $(this).data("id");
        Swal.fire({
            title: 'Delete price',
            text: 'Are you sure you want to delete',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes',
            cancelButtonText: 'No!!!'
        }).then((result) => {
            if (result.isConfirmed) {
                const formData = new FormData();
                formData.append('id', idPrice);
                axios.post('/system/remove-price', formData, {
                    headers: {
                        "Content-Type": "multipart/form-data",

                    }
                }).then(response => {
                    if (response.data.code == 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Deleted successfully',
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

                }).catch(error => {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'An error occurred, please try again',
                        confirmButtonText: 'OK'
                    });
                })
            } else {
                return;
            }
        })

    });
});