const API_URL = 'https://localhost:7249';

// Загрузка счетов в сайдбар
async function loadAccounts() {
    const res = await fetch(`${API_URL}/accounts`);
    const data = await res.json();
    const list = document.getElementById('accounts-list');
    list.innerHTML = data.map(acc => `
        <div class="card" onclick="showAccount('${acc.id}', '${acc.name}')">
            <div class="card-header">
                <span>${acc.name}</span>
                <span class="balance">${acc.total.toLocaleString()} ₽</span>
            </div>
        </div>
    `).join('');
}

// Показ истории транзакций
async function showAccount(id, name) {
    const area = document.getElementById('details-area');
    area.innerHTML = `
        <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:30px;">
            <h1>${name}</h1>
            <button class="btn-submit" style="width:auto; padding:10px 20px;" onclick="showForm('${id}')">+ Операция</button>
        </div>
        <div id="transactions-list">Загрузка...</div>
    `;

    const res = await fetch(`${API_URL}/transactions/${id}?page=1&limit=20`);
    const txs = await res.json();

    document.getElementById('transactions-list').innerHTML = txs.map(t => `
    <div class="tr-item">
        <div class="tr-info">
            <button class="btn-delete" onclick="deleteTransaction('${t.id}', '${id}', '${name}')" title="Удалить">
            &times;
            </button>
            <strong>${t.categoryName || 'Без категории'}</strong>
            <!-- Если описание есть, выводим его, если нет — ничего не рисуем -->
            ${t.description ? `<span class="tr-desc">${t.description}</span>` : ''}
            <small style="color: #bcc0c4; font-size: 0.75rem;">${new Date(t.createdTime).toLocaleDateString()}</small>
        </div>
        <span class="${t.type === 2 ? 'tr-amount-neg' : 'tr-amount-pos'}">
            ${t.type === 2 ? '-' : '+'}${t.value.toLocaleString()} ₽
        </span>
    </div>
`).join('');
}

// Показ формы добавления
async function showForm(accountId) {
    const area = document.getElementById('details-area');
    // Сначала загрузим категории для выпадашки
    const catRes = await fetch(`${API_URL}/categories`);
    const categories = await catRes.json();
    const currentName = document.querySelector('h1')?.innerText || "Счет";

    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Новая запись</h2>
            
            <div class="type-selector">
                <input type="radio" name="trType" id="type-exp" value="2" checked>
                <label class="type-btn" for="type-exp">Расход</label>
                
                <input type="radio" name="trType" id="type-inc" value="1">
                <label class="type-btn" for="type-inc">Доход</label>
            </div>

            <input type="number" id="amount" placeholder="0.00 ₽" step="0.01">
            
            <select id="category">
                ${categories.map(c => `<option value="${c.id}">${c.name}</option>`).join('')}
            </select>

            <input type="text" id="comment" placeholder="Комментарий">

            <button class="btn-submit" onclick="saveTransaction('${accountId}', '${currentName}')">Сохранить</button>
            <button onclick="showAccount('${accountId}', '${currentName}')" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// Отправка транзакции
async function saveTransaction(accountId, accountName) { // Добавили accountName
    const valInput = document.getElementById('amount').value;
    const typeInput = document.querySelector('input[name="trType"]:checked').value;
    const catInput = document.getElementById('category').value;
    const descInput = document.getElementById('comment').value;

    if (!valInput) return alert("Введите сумму");

    const payload = {
        accountId: accountId,
        transactionType: parseInt(typeInput),
        value: parseFloat(valInput),
        createdDay: new Date().toISOString().split('T')[0],
        description: descInput || null,
        relatedTransactionId: null,
        categoryId: catInput || null
    };

    try {
        const res = await fetch(`${API_URL}/transactions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        // ВАЖНО: Мы вызываем возврат в обоих случаях, если транзакция прошла
        if (res.ok) {
            await loadAccounts();
            showAccount(accountId, accountName); // Возвращаемся, используя переданное имя
        }
    } catch (e) {
        // Если была ошибка сети (CORS), но данные ушли
        await loadAccounts();
        showAccount(accountId, accountName);
    }
}

async function deleteTransaction(transactionId, accountId, accountName) {
    if (!confirm("Удалить эту операцию?")) return;

    try {
        const res = await fetch(`${API_URL}/transactions/${transactionId}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            // Обновляем всё: и баланс в сайдбаре, и список транзакций
            await loadAccounts();
            showAccount(accountId, accountName);
        } else {
            const err = await res.text();
            alert("Не удалось удалить: " + err);
        }
    } catch (e) {
        // Обработка CORS или ошибок сети (как в сохранении)
        console.error(e);
        await loadAccounts();
        showAccount(accountId, accountName);
    }
}

// Запуск
loadAccounts();