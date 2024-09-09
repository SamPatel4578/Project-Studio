document.addEventListener('DOMContentLoaded', function () {
    const cart = [];
    const cartItemsList = document.querySelector('.cart span');
    const totalPriceElement = document.getElementById('total-price');

    function updateCart() {
        let totalPrice = 0;
        cart.forEach(item => {
            totalPrice += item.price;
        });
        totalPriceElement.textContent = totalPrice.toFixed(2);
        cartItemsList.textContent = cart.length;
    }

    document.querySelectorAll('.add-to-cart').forEach(button => {
        button.addEventListener('click', function () {
            const product = this.closest('.product');
            const name = product.querySelector('h2').textContent;
            const price = parseFloat(product.querySelector('.new-price').textContent.replace('$', ''));
            cart.push({ name, price });
            updateCart();
        });
    });
});
