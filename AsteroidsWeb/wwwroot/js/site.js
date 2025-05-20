document.addEventListener("DOMContentLoaded", function () {
    const MAX_RANGE_DAYS = 7;
    const startInput = document.getElementById("startDate");
    const endInput = document.getElementById("endDate");
    const today = new Date();

    const formatDate = date => date.toISOString().split("T")[0];

    startInput.addEventListener("change", function () {
        const startStr = startInput.value;
        if (!startStr) return;

        const startDate = new Date(startStr);

        const maxEndDate = new Date(startDate);
        maxEndDate.setDate(startDate.getDate() + MAX_RANGE_DAYS);

        if (maxEndDate > today)
            maxEndDate.setTime(today.getTime());

        const maxStr = formatDate(maxEndDate);

        endInput.disabled = false;
        endInput.min = startStr;
        endInput.max = maxStr;

        endInput.value = maxStr;
    });
}); 

// Searching | Paging

window.addEventListener('DOMContentLoaded', () => {
    $('#asteroidsTable').DataTable({
        paging: true,
        searching: true,
        ordering: false
    });
});