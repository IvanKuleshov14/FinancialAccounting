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
            <div id="account-title-container">
                <h1 style="display:inline-block;">${name}</h1>
                <button class="btn-edit" onclick="enableEditAccount('${id}', '${name}')" title="Редактировать">✎</button>
            </div>
            <div style="display:flex; gap:10px;">
                <button class="btn-submit" style="background:#6c757d; width:auto; padding:10px 20px;" onclick="showTransferForm('${id}', '${name}')">⇄ Перевод</button>
                <button class="btn-submit" style="width:auto; padding:10px 20px;" onclick="showForm('${id}', '${name}')">+ Операция</button>
            </div>
        </div>
        <div id="transactions-list">Загрузка...</div>
    `;

    const res = await fetch(`${API_URL}/transactions/${id}?page=1&limit=20`);
    const txs = await res.json();

    document.getElementById('transactions-list').innerHTML = txs.map(t => {
        // 1. Проверяем, перевод это или нет
        const isTransfer = t.relatedTransactionId !== null;

        // 2. Определяем заголовок: Категория или спец-текст для перевода
        let displayTitle = t.categoryName || 'Без категории';
        if (isTransfer) {
            displayTitle = t.type === 2 ? '⇄ Перевод на другой счет' : '⇄ Перевод с другого счета';
        }

        // 3. Классы для суммы (Type 2 - Расход, Type 1 - Доход)
        const amountClass = t.type === 2 ? 'tr-amount-neg' : 'tr-amount-pos';
        const sign = t.type === 2 ? '-' : '+';

        return `
    <div class="tr-item">
        <button class="btn-delete" onclick="deleteTransaction('${t.id}', '${id}', '${name}')">&times;</button>
        
        <div class="tr-info">
            <strong>${displayTitle}</strong>
            <!-- Описание: если это перевод и оно пустое, можно вывести что-то по умолчанию -->
            <span class="tr-desc">${t.description || (isTransfer ? 'Внутренняя операция' : '')}</span>
            <small style="color: #bcc0c4; font-size: 0.75rem;">
                ${new Date(t.createdTime).toLocaleDateString()}
            </small>
        </div>
        <span class="${amountClass}">
            ${sign}${t.value.toLocaleString()} ₽
        </span>
    </div>
    `;
    }).join('');
}

// Форма создания транзакции
async function showForm(accountId) {
    const area = document.getElementById('details-area');
    // Сначала загрузим категории для выпадашки
    const catRes = await fetch(`${API_URL}/categories`);
    const categories = await catRes.json();
    const currentName = document.querySelector('h1')?.innerText || "Счет";

    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">${currentName}</h2>
            
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

// Сохранение транзакции
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

// Форма создания перевода
async function showTransferForm(fromId, fromName) {
    const area = document.getElementById('details-area');

    // Загружаем список всех счетов для выбора "Куда"
    const res = await fetch(`${API_URL}/Accounts`);
    const accounts = await res.json();

    // Исключаем текущий счет из списка "Куда"
    const targetAccounts = accounts.filter(a => a.id !== fromId);

    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Перевод со счета</h2>
            <h4 style="text-align:center; color:#0084ff; margin-bottom:20px;">${fromName}</h4>
            
            <label class="section-title">Куда перевести:</label>
            <select id="to-account">
                ${targetAccounts.map(a => `<option value="${a.id}">${a.name} (${a.total} ₽)</option>`).join('')}
            </select>

            <label class="section-title">Сумма:</label>
            <input type="number" id="transfer-amount" placeholder="0.00 ₽" step="0.01">
            
            <input type="text" id="transfer-comment" placeholder="Комментарий к переводу">

            <button class="btn-submit" onclick="submitTransfer('${fromId}', '${fromName}')">Подтвердить перевод</button>
            <button onclick="showAccount('${fromId}', '${fromName}')" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// Сохранение перевода
async function submitTransfer(fromId, fromName) {
    const toId = document.getElementById('to-account').value;
    const val = document.getElementById('transfer-amount').value;
    const desc = document.getElementById('transfer-comment').value;

    if (!val || val <= 0) return alert("Введите сумму");

    const payload = {
        fromAccountId: fromId,
        toAccountId: toId,
        value: parseFloat(val),
        createdDay: new Date().toISOString().split('T')[0], // Твой DateOnly
        description: desc || "Внутренний перевод"
    };

    try {
        const res = await fetch(`${API_URL}/Transactions/transfers`, { // Уточни путь к методу трансфера!
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            await loadAccounts();
            showAccount(fromId, fromName);
        } else {
            const err = await res.text();
            alert("Ошибка: " + err);
        }
    } catch (e) {
        // Обработка CORS если нужно
        await loadAccounts();
        showAccount(fromId, fromName);
    }
}

// Удаление транзакции
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

//Изменение имени счета
function enableEditAccount(id, oldName) {
    const container = document.getElementById('account-title-container');

    container.innerHTML = `
        <input type="text" id="edit-acc-name" class="edit-input" value="${oldName}" 
               onkeydown="if(event.key==='Enter') saveAccountName('${id}')">
        
        <div class="edit-group">
            <button class="btn-edit" id="btn-save-name" style="color: #28a745; font-size: 1.5rem; margin:0;">✔</button>
            <button class="btn-edit" id="btn-cancel-name" style="color: #888; font-size: 1.5rem; margin:0;">✖</button>
        </div>
    `;

    document.getElementById('btn-save-name').onclick = (e) => {
        e.stopPropagation(); // На всякий случай прерываем всплытие
        saveAccountName(id);
    };

    document.getElementById('btn-cancel-name').onclick = (e) => {
        e.stopPropagation();
        showAccount(id, oldName);
    };

    const input = document.getElementById('edit-acc-name');
    input.focus();
    input.select();
}

//Редактирование названия счета
async function saveAccountName(id) {
    const newName = document.getElementById('edit-acc-name').value;
    if (!newName) return;

    try {
        const res = await fetch(`${API_URL}/accounts/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: newName }) // Твой UpdateAccountDto
        });

        if (res.ok) {
            await loadAccounts(); // Обновляем сайдбар
            showAccount(id, newName); // Обновляем заголовок
        } else {
            alert("Ошибка при обновлении имени");
        }
    } catch (e) {
        console.error(e);
        // Обработка CORS если нужно
        await loadAccounts();
        showAccount(id, newName);
    }
}

// Запуск
loadAccounts();