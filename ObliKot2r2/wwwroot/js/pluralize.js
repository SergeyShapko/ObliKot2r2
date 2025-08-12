/**
 * Универсальная функция склонения украинских слов
 * @param {number} count - количество
 * @param {string[]} forms - массив форм слова [однина, множина, родовий відмінок]
 * @returns {string} - правильная форма слова
 */
function pluralize(count, forms) {
    if (count % 100 >= 11 && count % 100 <= 19) return forms[2];
    const lastDigit = count % 10;
    if (lastDigit === 1) return forms[0];
    if (lastDigit >= 2 && lastDigit <= 4) return forms[1];
    return forms[2];
}

// Конкретная функция для слова "запис"
function pluralizeRecords(count) {
    return pluralize(count, ['запис', 'записи', 'записів']);
}