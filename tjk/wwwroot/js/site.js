// TJK Delta Client Script

document.addEventListener('DOMContentLoaded', function () {
    // Smooth Scroll for Anchor Links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const targetId = this.getAttribute('href');
            if (targetId && targetId !== '#') {
                const targetElement = document.querySelector(targetId);
                if (targetElement) {
                    e.preventDefault();
                    targetElement.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    // Quick Quote Pre-fill for Contact Form
    const productSelectInput = document.querySelector('input[name="SelectedProduct"]');
    const urlParams = new URLSearchParams(window.location.search);
    const productParam = urlParams.get('product');
    if (productSelectInput && productParam) {
        productSelectInput.value = decodeURIComponent(productParam);
    }
});
