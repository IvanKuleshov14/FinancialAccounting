const API_URL = 'https://localhost:7249';

// Счета в левом сайдбаре
async function loadAccounts() {
    const res = await fetch(`${API_URL}/Accounts`);
    const data = await res.json();
    const list = document.getElementById('accounts-list');

    list.innerHTML = '';

    if (data.length === 0) {
        list.innerHTML = '<div style="color:#bbb; font-size:0.8rem; text-align:center;">Пока нет счетов</div>';
        return;
    }

    list.innerHTML = data.map(acc => {
        const progressValue = acc.targetProgress || 0;
        const barWidth = Math.min(progressValue, 100);

        return `
        <div class="card" onclick="showAccount('${acc.id}', '${acc.name}')">
            <div class="card-header">
                <span>${acc.name}</span>
                <span class="balance">${acc.total.toLocaleString()} ₽</span>
            </div>
            
            <!-- Если есть имя цели, рисуем БАР -->
            ${acc.targetName ? `
                <div class="progress-container">
                    <div class="progress-bar" style="width: ${barWidth}%"></div>
                </div>
                <div class="target-info">
                    <span style="font-size: 0.75rem; color: #0084ff; font-weight: 500;">🎯 ${acc.targetName}</span>
                    <span style="font-size: 0.75rem; font-weight: bold;">${Math.round(progressValue)}%</span>
                </div>
            ` : ''}
        </div>
        `;
    }).join('');
}

// Детализация счета
async function showAccount(id, name) {
    const area = document.getElementById('details-area');
    area.innerHTML = '<div style="text-align:center; margin-top:50px;">Загрузка...</div>';

    try {
        let catUrl = `${API_URL}/Categories`;
        if (currentFilterType !== 0) catUrl += `?type=${currentFilterType}`;

        const [accRes, txRes, catRes] = await Promise.all([
            fetch(`${API_URL}/Accounts/${id}`),
            fetch(`${API_URL}/transactions/${id}?page=1&limit=500`),
            fetch(catUrl)
        ]);

        const accData = await accRes.json();
        const allTxs = await txRes.json();
        const categories = await catRes.json();

        const periodTxs = allTxs.filter(t => {
            const d = new Date(t.createdTime);
            const m = currentFilterMonth === -1 || d.getMonth() === currentFilterMonth;
            const y = d.getFullYear() === currentFilterYear;
            return m && y;
        });

        const incomeSum = periodTxs.filter(t => t.type === 1).reduce((sum, t) => sum + t.value, 0);
        const expenseSum = periodTxs.filter(t => t.type === 2).reduce((sum, t) => sum + t.value, 0);

        const filteredTxs = periodTxs.filter(t => {
            const tMatch = currentFilterType === 0 || t.type === currentFilterType;
            const cMatch = currentFilterCategory === "all" || t.categoryName === currentFilterCategory;
            return tMatch && cMatch;
        });

        area.innerHTML = `
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:30px;">
                <div id="account-title-container" style="display: flex; align-items: center; gap: 10px;">
                    <h1 style="margin:0;">${name}</h1>
                    <button class="btn-edit" onclick="enableEditAccount('${id}', '${name}')">✎</button>
                </div>
                <div style="display:flex; gap:10px; align-items: center;">
                    <button class="btn-submit" style="background:#6c757d; width:auto; padding:10px 20px;" onclick="showAccountTargetForm('${id}', '${name}')">🎯 Цель</button>
                    <button class="btn-submit" style="background:#6c757d; width:auto; padding:10px 20px;" onclick="showTransferForm('${id}', '${name}')">⇄ Перевод</button>
                    <button class="btn-submit" style="background:#28a745; width:auto; padding:10px 20px;" onclick="showForm('${id}', '${name}')">+ Операция</button>
                    <div class="dropdown">
                        <button class="btn-more" onclick="toggleAccountMenu()">⋮</button>
                        <div id="account-dropdown" class="dropdown-content">
                            <button onclick="deleteAccount('${id}')">🗑 Удалить счет</button>
                        </div>
                    </div>
                </div>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 30px;">
                <div class="form-card" onclick="toggleAccountTypeFilter('${id}', '${name}', 1)" 
                     style="border-left: 5px solid #28a745; cursor: pointer; border: ${currentFilterType === 1 ? '2px solid #28a745' : '1px solid #eee'}; background: ${currentFilterType === 1 ? '#f1fdf7' : 'white'}; padding: 15px; margin:0;">
                    <small style="color:#888; text-transform:uppercase; font-size:0.7rem;">Доходы ${currentFilterType === 1 ? '●' : ''}</small>
                    <div style="font-size: 1.3rem; font-weight: bold; color: #28a745;">+${incomeSum.toLocaleString()} ₽</div>
                </div>
                <div class="form-card" onclick="toggleAccountTypeFilter('${id}', '${name}', 2)" 
                     style="border-left: 5px solid #dc3545; cursor: pointer; border: ${currentFilterType === 2 ? '2px solid #dc3545' : '1px solid #eee'}; background: ${currentFilterType === 2 ? '#fff5f5' : 'white'}; padding: 15px; margin:0;">
                    <small style="color:#888; text-transform:uppercase; font-size:0.7rem;">Расходы ${currentFilterType === 2 ? '●' : ''}</small>
                    <div style="font-size: 1.3rem; font-weight: bold; color: #dc3545;">-${expenseSum.toLocaleString()} ₽</div>
                </div>
            </div>

            <div style="display: flex !important; justify-content: space-between !important; align-items: center !important; margin: 25px 0 15px 0 !important; width: 100% !important; min-height: 40px !important;">
    <h4 style="margin: 0 !important; font-size: 1.1rem; white-space: nowrap !important; display: flex !important; align-items: center !important;">
        ${currentFilterType === 0 ? 'История операций' : currentFilterType === 1 ? 'Только доходы' : 'Только расходы'}
    </h4>
    <div style="display: flex !important; gap: 10px !important; align-items: center !important;">
        <select class="select-inline" onchange="changeAccountFilter('${id}', '${name}', this.value, null, null)" 
                style="height: 38px !important; border-radius: 8px !important; margin: 0 !important; box-sizing: border-box !important;">
            <option value="-1" ${currentFilterMonth === -1 ? 'selected' : ''}>Все месяцы</option>
            ${['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'].map((m, i) => `<option value="${i}" ${i === currentFilterMonth ? 'selected' : ''}>${m}</option>`).join('')}
        </select>
        <select class="select-inline" onchange="changeAccountFilter('${id}', '${name}', null, this.value, null)" 
                style="height: 38px !important; border-radius: 8px !important; margin: 0 !important; box-sizing: border-box !important;">
            <option value="2025" ${currentFilterYear === 2025 ? 'selected' : ''}>2025</option>
            <option value="2026" ${currentFilterYear === 2026 ? 'selected' : ''}>2026</option>
        </select>
        <select class="select-inline" onchange="changeAccountFilter('${id}', '${name}', null, null, this.value)" 
                style="height: 38px !important; border-radius: 8px !important; width: 140px !important; margin: 0 !important; box-sizing: border-box !important;">
            <option value="all">Все категории</option>
            ${categories.map(c => `<option value="${c.name || c.Name}" ${currentFilterCategory === (c.name || c.Name) ? 'selected' : ''}>${c.name || c.Name}</option>`).join('')}
        </select>
    </div>
</div>

            <div id="transactions-list">
                ${filteredTxs.length > 0 ? filteredTxs.map(t => {
            const isTransfer = t.relatedTransactionId !== null;
                    const title = isTransfer ? (t.type === 2 ? '⇄ Между своими счетами' : '⇄ Между своими счетами') : (t.categoryName || 'Без категории');
            return `
                        <div class="tr-item">
                            <button class="btn-delete" onclick="deleteTransaction('${t.id}', '${id}', '${name}')">&times;</button>
                            <div class="tr-info">
                                <strong>${title}</strong>
                                <span class="tr-desc">${t.description || ''}</span>
                                <small style="color: #bcc0c4;">${new Date(t.createdTime).toLocaleDateString()}</small>
                            </div>
                            <span class="${t.type === 2 ? 'tr-amount-neg' : 'tr-amount-pos'}">
                                ${t.type === 2 ? '-' : '+'}${t.value.toLocaleString()} ₽
                            </span>
                        </div>`;
        }).join('') : '<p class="text-center text-muted p-5">Транзакций не найдено</p>'}
            </div>
        `;

    } catch (e) { console.error(e); area.innerHTML = "<h2>Ошибка загрузки счета</h2>"; }
}

// Фильтры для детализации счета
function changeAccountFilter(id, name, month, year, category) {
    if (month !== null) currentFilterMonth = parseInt(month);
    if (year !== null) currentFilterYear = parseInt(year);
    if (category !== null) currentFilterCategory = category;
    showAccount(id, name);
}

// Фильтр через клик по карточкам сумм транзакций (доходы и расходы)
function toggleAccountTypeFilter(id, name, type) {
    currentFilterType = (currentFilterType === type) ? 0 : type;
    currentFilterCategory = "all";
    showAccount(id, name);
}

let allCategories = [];

// Форма создания транзакции
async function showForm(accountId, accountName) {
    const area = document.getElementById('details-area');

    try {
        const catRes = await fetch(`${API_URL}/Categories`);
    } catch (e) {
        console.error("Ошибка загрузки категорий", e);
    }

    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Новая транзакция</h2>
            <h4 style="text-align:center; color:#0084ff; margin-bottom:20px;">${accountName}</h4>
            
            <div class="type-selector">
                <input type="radio" name="trType" id="type-exp" value="2" checked onchange="updateCategoryList(2)">
                <label class="type-btn" for="type-exp">Расход</label>
                
                <input type="radio" name="trType" id="type-inc" value="1" onchange="updateCategoryList(1)">
                <label class="type-btn" for="type-inc">Доход</label>
            </div>

            <label class="section-title">Сумма (₽):</label>
            <input type="number" id="amount" placeholder="0.00" step="0.01" autofocus>
            
            <label class="section-title">Категория:</label>
<div class="input-row" style="display: flex; gap: 8px; align-items: center;">
    <select id="category-select" style="flex-grow: 1; height: 45px; margin: 0;">
    </select>

    <button type="button" class="btn-inline-add"
            onclick="showEditCategoryForm('${accountId}', '${accountName}')" 
            style="
                background: #f0f2f5; 
                color: #65676b; 
                border: 1px solid #ddd; 
                margin-right: 12px !important; /* ОТСТУП ЗДЕСЬ */
                width: 45px; height: 45px; flex-shrink: 0;
            " title="Редактировать категорию">✎</button>

    <button type="button" class="btn-inline-add" 
            onclick="showCreateCategoryForm('${accountId}', '${accountName}')" 
            style="
                background: #0084ff; 
                color: white; 
                border: none; 
                width: 45px; height: 45px; flex-shrink: 0;
            ">+</button>
</div>

            <label class="section-title">Комментарий:</label>
            <input type="text" id="comment" placeholder="На что потратили?">

            <button class="btn-submit" onclick="saveTransaction('${accountId}', '${accountName}')">Сохранить</button>
            <button onclick="showAccount('${accountId}', '${accountName}')" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;

    // Инициализируем список категорий для типа "Расход" (2) по умолчанию
    updateCategoryList(2);
}

// Функция фильтрации категорий по типу
async function updateCategoryList(type) {
    const select = document.getElementById('category-select');
    if (!select) return;

    try {
        const res = await fetch(`${API_URL}/Categories?type=${type}`);
        const categories = await res.json();

        if (categories.length === 0) {
            select.innerHTML = '<option value="">Нет категорий для этого типа</option>';
        } else {
            select.innerHTML = categories.map(c =>
                `<option value="${c.id || c.Id}">${c.name || c.Name}</option>`
            ).join('');
        }
    } catch (e) {
        console.error("Ошибка загрузки категорий по типу:", e);
        select.innerHTML = '<option value="">Ошибка загрузки</option>';
    }
}

// Сохранение транзакции
async function saveTransaction(accountId, accountName) {
    const valInput = document.getElementById('amount').value;
    const typeInput = document.querySelector('input[name="trType"]:checked').value;
    const catInput = document.getElementById('category-select').value;
    const categoryId = catInput === "" ? null : catInput;
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

        if (res.ok) {
            await loadAccounts();
            await loadTotalBalance();
            showAccount(accountId, accountName);
        }
    } catch (e) {
        await loadAccounts();
        await loadTotalBalance();
        showAccount(accountId, accountName);
    }
}

// Форма создания перевода
async function showTransferForm(fromId, fromName) {
    const area = document.getElementById('details-area');

    const res = await fetch(`${API_URL}/Accounts`);
    const accounts = await res.json();

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
        createdDay: new Date().toISOString().split('T')[0], 
        description: desc || "Внутренний перевод"
    };

    try {
        const res = await fetch(`${API_URL}/Transactions/transfers`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            await loadAccounts();
            await loadTotalBalance();
            showAccount(fromId, fromName);
        } else {
            const err = await res.text();
            alert("Ошибка: " + err);
        }
    } catch (e) {
        await loadAccounts();
        await loadTotalBalance();
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
            await loadAccounts();
            await loadTotalBalance();
            showAccount(accountId, accountName);
        } else {
            const err = await res.text();
            alert("Не удалось удалить: " + err);
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        await loadTotalBalance();
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
        e.stopPropagation();
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

//Сохранение имени счета
async function saveAccountName(id) {
    const newName = document.getElementById('edit-acc-name').value;
    if (!newName) return;

    try {
        const res = await fetch(`${API_URL}/accounts/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: newName }) 
        });

        if (res.ok) {
            await loadAccounts(); 
            await loadTotalBalance();
            showAccount(id, newName); 
        } else {
            alert("Ошибка при обновлении имени");
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        await loadTotalBalance();
        showAccount(id, newName);
    }
}

// Переключение видимости меню
function toggleAccountMenu() {
    document.getElementById("account-dropdown").classList.toggle("show");
}

// Закрытие меню при клике в любом другом месте
window.onclick = function (event) {
    if (!event.target.matches('.btn-more')) {
        const dropdowns = document.getElementsByClassName("dropdown-content");
        for (let i = 0; i < dropdowns.length; i++) {
            let openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}

// Удаление счета
async function deleteAccount(id) {
    if (!confirm("Вы уверены, что хотите удалить этот счет со всеми транзакциями? Это действие нельзя отменить.")) return;

    try {
        const res = await fetch(`${API_URL}/Accounts/${id}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            await loadAccounts();
            await loadTotalBalance();
            document.getElementById('details-area').innerHTML = `
                <div style="text-align: center; color: #888; margin-top: 100px;">
                    <h2>Счет удален</h2>
                    <p>Выберите другой счет слева</p>
                </div>`;
        } else {
            alert("Ошибка при удалении счета");
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        await loadTotalBalance();
        location.reload();
    }
}

// Форма создания счета
function showCreateAccountForm() {
    const area = document.getElementById('details-area');
    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Новый счет</h2>
            
            <label class="section-title">Название счета:</label>
            <input type="text" id="new-acc-name" placeholder="Например: Наличные или Карта" autofocus>

            <label class="section-title">Начальный баланс (₽):</label>
            <input type="number" id="new-acc-total" value="0" step="0.01">

            <button class="btn-submit" onclick="submitCreateAccount()">Создать счет</button>
            <button onclick="location.reload()" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// Сохранение счета
async function submitCreateAccount() {
    const nameInput = document.getElementById('new-acc-name');
    const totalInput = document.getElementById('new-acc-total');

    if (!nameInput.value.trim()) return alert("Введите название");

    const payload = {
        name: nameInput.value.trim(),
        userId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        total: parseFloat(totalInput.value) || 0
    };

    try {
        const res = await fetch(`${API_URL}/Accounts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            try {
                const createdAccount = await res.json();
                await loadAccounts();
                await loadTotalBalance();
                showAccount(createdAccount.id, createdAccount.name);
            } catch {
                await loadAccounts();
                await loadTotalBalance();
                document.getElementById('details-area').innerHTML = "<h2>Счет создан</h2>";
            }
        } else {
            const err = await res.text();
            alert("Сервер не принял UserId: " + err);
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        await loadTotalBalance();
    }
}

// Форма для создания привязанной цели
async function showAccountTargetForm(accountId, accountName) {
    const area = document.getElementById('details-area');
    area.innerHTML = '<div class="text-center mt-5">Загрузка данных цели...</div>';

    try {
        const res = await fetch(`${API_URL}/Accounts/${accountId}`);
        const acc = await res.json();

        if (!acc.targetName) {
            renderCreateTargetForm(accountId, accountName);
        } else {
            renderTargetDetailsCard(acc);
        }
    } catch (e) {
        console.error(e);
        area.innerHTML = "Ошибка загрузки цели";
    }
}

// Вспомогательная функция: Форма создания
function renderCreateTargetForm(accountId, accountName) {
    const area = document.getElementById('details-area');
    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Установить цель</h2>
            <p style="text-align:center; color:#888;">Для счета: ${accountName}</p>
            <input type="text" id="target-name" placeholder="Название (напр. На отпуск)">
            <input type="number" id="target-goal" placeholder="Сумма цели (₽)">
            <button class="btn-submit" onclick="submitAccountTarget('${accountId}', '${accountName}')">Создать</button>
            <button class="btn-submit" style="background:none; color:#888;" onclick="showAccount('${accountId}', '${accountName}')">Назад</button>
        </div>
    `;
}

// Вспомогательная функция: Карточка деталей
function renderTargetDetailsCard(acc) {
    const area = document.getElementById('details-area');
    const percent = Math.min(Math.round(acc.targetProgress || 0), 100);

    area.innerHTML = `
        <div class="form-card" style="position: relative !important; padding-top: 50px !important;">
            
            <button onclick="deleteAccountTarget('${acc.accountTargetId}', '${acc.id}', '${acc.name}')" 
    style="
        position: absolute !important;
        top: 20px !important;
        right: 20px !important;
        width: 40px !important;
        height: 40px !important;
        background: #fff5f5 !important; /* Нежно-красный фон */
        border: 1px solid #ffc1c1 !important; /* Граница */
        color: #dc3545 !important; /* Ярко-красная корзина */
        border-radius: 8px !important;
        display: flex !important;
        align-items: center !important;
        justify-content: center !important;
        cursor: pointer !important;
        z-index: 999 !important;
        transition: 0.2s !important;
        font-size: 1.2rem !important;
    " 
    onmouseover="this.style.background='#dc3545'; this.style.color='white'" 
    onmouseout="this.style.background='#fff5f5'; this.style.color='#dc3545'"
    title="Удалить цель">
    🗑
</button>

            <h2 style="margin-bottom:10px;">🎯 ${acc.targetName}</h2>
            <div style="font-size: 1.1rem; margin-bottom: 20px; color: #65676b;">
                Счет: <strong>${acc.name}</strong>
            </div>

            <div class="progress-container" style="height: 15px; margin-bottom: 10px;">
                <div class="progress-bar" style="width: ${percent}%"></div>
            </div>
            
            <div style="display:flex; justify-content:space-between; margin-bottom:30px;">
                <span style="font-weight:bold; font-size: 1.2rem;">${percent}%</span>
                <span>${acc.total.toLocaleString()} / ${acc.targetGoal?.toLocaleString()} ₽</span>
            </div>

            <div style="display:flex; gap:10px;">
                <button class="btn-submit" style="background:#f0f2f5; color:#1a1a1a; width: auto; padding: 10px 20px;" 
                    onclick="showEditTargetForm('${acc.id}', '${acc.name}', '${acc.targetName}', ${acc.targetGoal}, '${acc.accountTargetId}')">
                    Изменить цель
                </button>
                <button class="btn-submit" style="background:#6c757d; width: auto; padding: 10px 20px;" 
                    onclick="showAccount('${acc.id}', '${acc.name}')">
                    Назад
                </button>
            </div>
        </div>
    `;
}

// Сохранение привязанной цели
async function submitAccountTarget(accountId, accountName) {
    const name = document.getElementById('target-name').value;
    const goal = document.getElementById('target-goal').value;

    if (!name || !goal) return alert("Заполните все поля");

    const payload = {
        name: name,
        goal: parseFloat(goal)
    };

    try {
        const res = await fetch(`${API_URL}/Accounts/${accountId}/account_targets`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            await loadAccounts(); 
            await loadTotalBalance();
            showAccount(accountId, accountName);
        } else {
            alert("Ошибка при установке цели");
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        await loadTotalBalance();
        showAccount(accountId, accountName);
    }
}

// Удаление привязанной цели
async function deleteAccountTarget(targetId, accountId, accountName) {
    if (!confirm("Удалить цель? Прогресс будет скрыт, но баланс счета не изменится.")) return;

    try {
        const res = await fetch(`${API_URL}/AccountTargets/${targetId}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            await loadAccounts(); 
            await loadTotalBalance();
            showAccount(accountId, accountName);
        } else {
            alert("Ошибка при удалении цели с сервера");
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        await loadTotalBalance();
        showAccount(accountId, accountName);
    }
}

// Форма создания категории
function showCreateCategoryForm(accountId, accountName) {
    const area = document.getElementById('details-area');
    const currentType = parseInt(document.querySelector('input[name="trType"]:checked').value);
    const typeText = currentType === 1 ? "доход" : "расход";

    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Новая категория</h2>
            <p style="text-align:center; color:#888;">Тип: <strong>${typeText}</strong></p>
            
            <label class="section-title">Название:</label>
            <input type="text" id="new-cat-name" placeholder="Например: Аптека или Фриланс" autofocus>

            <button class="btn-submit" onclick="submitCreateCategory('${accountId}', '${accountName}', ${currentType})">Создать</button>
            <button onclick="showForm('${accountId}', '${accountName}')" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// Сохранение категории
async function submitCreateCategory(accountId, accountName, type) {
    const name = document.getElementById('new-cat-name').value;
    if (!name) return alert("Введите название");

    const payload = {
        name: name,
        type: type 
    };

    try {
        const res = await fetch(`${API_URL}/Categories`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            showForm(accountId, accountName);
            setTimeout(() => updateCategoryList(type), 100);
        } else {
            alert("Ошибка при создании категории");
        }
    } catch (e) {
        console.error(e);
        showForm(accountId, accountName);
    }
}

// Удаление категории
async function deleteCurrentCategory(accountId, accountName) {
    const select = document.getElementById('category-select');
    const categoryId = select.value;
    const categoryName = select.options[select.selectedIndex]?.text;

    if (!categoryId || categoryId === "") return alert("Выберите категорию для удаления");
    if (!confirm(`Скрыть категорию "${categoryName}"? Она останется в старых транзакциях, но исчезнет из этого списка.`)) return;

    try {
        const res = await fetch(`${API_URL}/Categories/${categoryId}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            // Определяем текущий тип (1 или 2), чтобы обновить список
            const currentType = parseInt(document.querySelector('input[name="trType"]:checked').value);
            await updateCategoryList(currentType);
        } else {
            alert("Ошибка при удалении категории");
        }
    } catch (e) {
        console.error(e);
        // Fallback для CORS
        location.reload();
    }
}

// Форма редактирования цели
function showEditTargetForm(accountId, accountName, oldName, oldGoal, targetId) {
    const area = document.getElementById('details-area');
    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Изменить цель</h2>
            
            <label class="section-title">Название:</label>
            <input type="text" id="edit-target-name" value="${oldName}">

            <label class="section-title">Новая сумма цели (₽):</label>
            <input type="number" id="edit-target-goal" value="${oldGoal}">

            <button class="btn-submit" onclick="submitUpdateTarget('${targetId}', '${accountId}', '${accountName}')">Сохранить изменения</button>
            <button onclick="showAccountTargetForm('${accountId}', '${accountName}')" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// Сохранение редактирования цели
async function submitUpdateTarget(targetId, accountId, accountName) {
    const newName = document.getElementById('edit-target-name').value;
    const newGoal = document.getElementById('edit-target-goal').value;

    if (!newName || !newGoal) return alert("Заполните поля");

    const payload = {
        name: newName,
        goal: parseFloat(newGoal)
    };

    try {
        const res = await fetch(`${API_URL}/AccountTargets/${targetId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            await loadAccounts(); 
            await loadTotalBalance();
            showAccount(accountId, accountName); 
        } else {
            alert("Ошибка при обновлении цели");
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        await loadTotalBalance();
        showAccount(accountId, accountName);
    }
}

// Форма редактирования категории
function showEditCategoryForm(accountId, accountName) {
    const select = document.getElementById('category-select');
    const categoryId = select.value;
    const categoryName = select.options[select.selectedIndex]?.text;

    if (!categoryId || categoryId === "") return alert("Выберите категорию");

    const area = document.getElementById('details-area');
    area.innerHTML = `
        <div class="form-card" style="position: relative;">
            <h2 style="text-align:center; margin-bottom:20px;">Редактировать категорию</h2>
            
            <label class="section-title">Название категории:</label>
            <input type="text" id="edit-cat-name" value="${categoryName}" autofocus>

            <div style="display: flex; gap: 10px; margin-top: 20px;">
                <button class="btn-submit" onclick="submitUpdateCategory('${categoryId}', '${accountId}', '${accountName}')">Сохранить</button>
                
                <button class="btn-submit" style="background: #fff5f5; color: #dc3545; border: 1px solid #ffc1c1; width: 60px;" 
                        onclick="deleteCurrentCategoryConfirm('${categoryId}', '${accountId}', '${accountName}')" title="Удалить">🗑</button>
            </div>

            <button onclick="showForm('${accountId}', '${accountName}')" style="background:none; border:none; color:#888; width:100%; margin-top:15px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// Сохранение редактированной категории
async function submitUpdateCategory(categoryId, accountId, accountName) {
    const newName = document.getElementById('edit-cat-name').value;
    if (!newName) return alert("Введите название");

    try {
        const res = await fetch(`${API_URL}/Categories/${categoryId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: newName })
        });

        if (res.ok) {
            showForm(accountId, accountName);
        }
    } catch (e) { console.error(e); showForm(accountId, accountName); }
}

// Удаление категории
async function deleteCurrentCategoryConfirm(categoryId, accountId, accountName) {
    if (!confirm("Скрыть эту категорию из списка?")) return;

    try {
        const res = await fetch(`${API_URL}/Categories/${categoryId}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            showForm(accountId, accountName);
        }
    } catch (e) { console.error(e); showForm(accountId, accountName); }
}

// Цели в левом сайдбаре
async function loadTargets() {
    try {
        const res = await fetch(`${API_URL}/Targets`);
        const targets = await res.json();
        const list = document.getElementById('targets-list');

        list.innerHTML = '';

        if (targets.length === 0) {
            list.innerHTML = '<div style="color:#bbb; font-size:0.8rem; text-align:center;">Пока нет целей</div>';
            return;
        }

        list.innerHTML = targets.map(t => {
            const progressValue = t.progress ?? 0;
            const barWidth = Math.min(progressValue, 100);

            return `
                <div class="card" onclick="showTargetDetails('${t.id}', '${t.name}')">
                    <div class="card-header">
                        <span style="font-weight: 500;">${t.name}</span>
                        <span style="color: #28a745; font-weight: bold;">${t.total.toLocaleString()} ₽</span>
                    </div>
                    
                    <div class="progress-container" style="height: 6px; margin-top: 10px;">
                        <div class="progress-bar" style="width: ${barWidth}%; background: #28a745;"></div>
                    </div>
                    
                    <div class="target-info" style="margin-top: 5px;">
                        <span style="font-size: 0.7rem; color: #888;">Цель: ${t.goal.toLocaleString()} ₽</span>
                        <span style="font-size: 0.7rem; font-weight: bold;">${Math.round(progressValue)}%</span>
                    </div>
                </div>
            `;
        }).join('');
    } catch (e) {
        console.error("Ошибка загрузки автономных целей:", e);
    }
}

// Детализация цели
async function showTargetDetails(id, name) {
    const area = document.getElementById('details-area');
    area.innerHTML = '<div class="text-center mt-5">Обработка...</div>';

    try {
        const res = await fetch(`${API_URL}/Targets/${id}`);
        if (!res.ok) throw new Error(`Ошибка сервера: ${res.status}`);

        const target = await res.json();
        console.log("Пришли данные:", target); 

        const tId = target.id || target.Id || id;
        const tName = target.name || target.Name || name;
        const tTotal = target.total ?? target.Total ?? 0;
        const tGoal = target.goal ?? target.Goal ?? 0;
        const tProgress = target.progress ?? target.Progress ?? 0;

        const percent = Math.min(Math.round(tProgress), 100);

        area.innerHTML = `
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:30px;">
                <h1 style="margin:0;">${tName}</h1>
                <div style="display:flex; gap:10px;">
   <button class="btn-submit" style="background:#28a745; width:auto; padding:10px 20px;"
    onclick="openDepositTargetForm('${tId}', '${tName}')">+ Операция</button>

    <button class="btn-inline-add"
            onclick="showEditAutonomousTargetForm('${tId}', '${tName}', ${tGoal})" 
            style="background: #f0f2f5; color: #65676b; border: 1px solid #ddd; width: 45px; height: 45px;" 
            title="Редактировать">✎</button>
            
    <button class="btn-inline-del" onclick="deleteTarget('${tId}')" style="margin:0; width: 45px; height: 45px;">🗑</button>
</div>
            </div>

            <div class="form-card" style="border-top: 5px solid #28a745; margin-bottom: 30px; background: white; padding: 25px; border-radius: 15px; box-shadow: 0 4px 12px rgba(0,0,0,0.05);">
                <div style="display:flex; justify-content:space-between; align-items:flex-end; margin-bottom:15px;">
                    <div>
                        <small style="color:#888; text-transform:uppercase;">Накоплено</small>
                        <div style="font-size: 2rem; font-weight: 800; color: #28a745;">${Number(tTotal).toLocaleString()} ₽</div>
                    </div>
                    <div style="text-align:right;">
                        <small style="color:#888; text-transform:uppercase;">Цель</small>
                        <div style="font-size: 1.2rem; font-weight: 600;">${Number(tGoal).toLocaleString()} ₽</div>
                    </div>
                </div>

                <div style="width: 100%; height: 16px; background:#e9ecef; border-radius: 10px; overflow: hidden;">
                    <div style="width: ${percent}%; height: 100%; background: #28a745; transition: width 0.5s;"></div>
                </div>
                <div style="text-align:center; margin-top:10px; font-weight:bold; color:#28a745;">${percent}% завершено</div>
            </div>

            <div class="section-title" style="margin-top: 40px; color: #65676b; font-size: 0.9rem; text-transform: uppercase;">История пополнений</div>
            <div id="target-history-list">
            </div>
        `;

        // Загружаем транзакции для этой цели
        loadTargetTransactions(tId);

    } catch (err) {
        console.error("Ошибка в блоке отрисовки:", err);
        area.innerHTML = `
            <div style="padding: 20px; border: 1px solid #ffc1c1; background: #fff5f5; border-radius: 10px; color: #dc3545;">
                <h4>Ошибка отрисовки</h4>
                <p>${err.message}</p>
                <small>Проверь консоль браузера (F12) для деталей.</small>
            </div>`;
    }
}

// Форма создания цели
function showCreateTargetForm() {
    const area = document.getElementById('details-area');
    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Новая копилка</h2>
            
            <label class="section-title">На что копим?</label>
            <input type="text" id="target-name" placeholder="Например: Подушка безопасности" autofocus>

            <label class="section-title">Сколько уже отложено (₽):</label>
            <input type="number" id="target-total" value="0" step="0.01">

            <label class="section-title">Цель (₽):</label>
            <input type="number" id="target-goal" placeholder="100000" step="0.01">

            <button class="btn-submit" onclick="submitCreateTarget()">Создать копилку</button>
            <button onclick="location.reload()" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// Сохранение цели
async function submitCreateTarget() {
    const nameInput = document.getElementById('target-name');
    const goalInput = document.getElementById('target-goal');
    const totalInput = document.getElementById('target-total') || { value: 0 };

    if (!nameInput.value.trim() || !goalInput.value) {
        alert("Заполните название и сумму цели");
        return;
    }

    const payload = {
        userId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        name: nameInput.value.trim(),
        total: parseFloat(totalInput.value) || 0,
        goal: parseFloat(goalInput.value)
    };

    try {
        const res = await fetch(`${API_URL}/Targets`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            await loadTargets(); 

            try {
                const created = await res.json();
                showTargetDetails(created.id, created.name);
            } catch {
                document.getElementById('details-area').innerHTML = "<h3>Копилка создана!</h3>";
            }
        } else {
            const err = await res.text();
            alert("Ошибка: " + err);
        }
    } catch (e) {
        console.error(e);
        await loadTargets();
    }
}

// Форма редактирования цели
function showEditAutonomousTargetForm(id, oldName, oldGoal) {
    const area = document.getElementById('details-area');

    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Изменить копилку</h2>
            
            <label class="section-title">Название:</label>
            <input type="text" id="edit-target-name" value="${oldName}" autofocus>

            <label class="section-title">Сумма цели (₽):</label>
            <input type="number" id="edit-target-goal" value="${oldGoal}" step="0.01">

            <div style="display: flex; gap: 10px; margin-top: 20px;">
                <button class="btn-submit" style="background:#28a745;" onclick="submitUpdateAutonomousTarget('${id}')">Сохранить</button>
                <button class="btn-submit" style="background:#6c757d;" onclick="showTargetDetails('${id}', '${oldName}')">Отмена</button>
            </div>
        </div>
    `;
}

// Сохранение редактированной цели
async function submitUpdateAutonomousTarget(id) {
    const newName = document.getElementById('edit-target-name').value;
    const newGoal = document.getElementById('edit-target-goal').value;

    if (!newName || !newGoal) return alert("Заполните все поля");

    const payload = {
        name: newName,
        goal: parseFloat(newGoal)
    };

    try {
        const res = await fetch(`${API_URL}/Targets/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            await loadTargets();
            showTargetDetails(id, newName);
        } else {
            alert("Не удалось сохранить изменения");
        }
    } catch (e) {
        console.error("Ошибка при обновлении цели:", e);
        await loadTargets();
        showTargetDetails(id, newName);
    }
}

// Удаление цели
async function deleteTarget(id) {
    if (!confirm("Вы уверены, что хотите удалить эту копилку? Все данные о накоплениях будут потеряны.")) {
        return;
    }

    try {
        const res = await fetch(`${API_URL}/Targets/${id}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            await loadTargets();

            document.getElementById('details-area').innerHTML = `
                <div class="text-center text-muted mt-5">
                    <h2>Копилка удалена</h2>
                    <p>Выберите другую цель или счет слева</p>
                </div>
            `;
        } else {
            const err = await res.text();
            alert("Ошибка при удалении: " + err);
        }
    } catch (e) {
        console.error("Ошибка удаления:", e);
        await loadTargets();
        document.getElementById('details-area').innerHTML = "<h2>Готово</h2>";
    }
}

// Новая операция с целью
function openDepositTargetForm(targetId, targetName) {
    const area = document.getElementById('details-area');

    area.innerHTML = `
        <div class="form-card" style="border-top: 5px solid #28a745;">
            <h2 style="text-align:center; margin-bottom:20px;">Операция с копилкой</h2>
            <p style="text-align:center; color:#888; margin-bottom:20px;">Цель: <strong>${targetName}</strong></p>
            
            <div class="type-selector" style="margin-bottom: 30px;">
                <input type="radio" name="depType" id="type-dep-inc" value="1" checked>
                <label class="type-btn" for="type-dep-inc" style="border-color: #28a745; color: #28a745;">Положить</label>
                
                <input type="radio" name="depType" id="type-dep-exp" value="2">
                <label class="type-btn" for="type-dep-exp" style="border-color: #dc3545; color: #dc3545;">Забрать</label>
            </div>

            <label class="section-title">Сумма (₽):</label>
            <input type="number" id="deposit-value" placeholder="0.00" step="0.01" autofocus 
                   style="font-size: 2.5rem; text-align: center; font-weight: 800; border: none; outline: none; width: 100%;">

            <label class="section-title" style="margin-top:20px;">Описание:</label>
            <input type="text" id="deposit-desc" placeholder="На что или откуда?">

            <div style="margin-top: 30px; display: flex; gap: 10px;">
                <button class="btn-submit" style="background:#28a745;" onclick="submitTargetDeposit('${targetId}', '${targetName}')">Подтвердить</button>
                <button class="btn-submit" style="background:#f0f2f5; color:#888;" onclick="showTargetDetails('${targetId}', '${targetName}')">Отмена</button>
            </div>
        </div>
    `;

    // Смена цвета суммы при переключении типа (для визуала)
    document.getElementById('type-dep-inc').onchange = () => document.getElementById('deposit-value').style.color = '#28a745';
    document.getElementById('type-dep-exp').onchange = () => document.getElementById('deposit-value').style.color = '#dc3545';

    // По умолчанию зеленый
    document.getElementById('deposit-value').style.color = '#28a745';
}

// Сохранение операции с целью 
async function submitTargetDeposit(targetId, targetName) {
    const valInput = document.getElementById('deposit-value').value;
    const descInput = document.getElementById('deposit-desc').value;
    const typeInput = document.querySelector('input[name="depType"]:checked').value;

    if (!valInput || valInput <= 0) return alert("Введите сумму");

    const payload = {
        targetId: targetId,
        type: parseInt(typeInput), 
        value: parseFloat(valInput),
        createdDay: new Date().toISOString().split('T')[0],
        description: descInput || (parseInt(typeInput) === 1 ? "Пополнение" : "Снятие")
    };

    try {
        const res = await fetch(`${API_URL}/TargetTransactions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            await loadTargets();
            showTargetDetails(targetId, targetName);
        } else {
            const err = await res.text();
            alert("Ошибка: " + err);
        }
    } catch (e) {
        console.error(e);
        await loadTargets();
        showTargetDetails(targetId, targetName);
    }
}

// Удаление операции с целью
async function deleteTargetTransaction(transactionId, targetId, targetName) {
    if (!confirm("Удалить эту запись из истории копилки?")) return;

    try {
        const res = await fetch(`${API_URL}/TargetTransactions/${transactionId}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            await loadTargets();
            showTargetDetails(targetId, targetName);
        } else {
            alert("Не удалось удалить транзакцию");
        }
    } catch (e) {
        console.error(e);
        await loadTargets();
        showTargetDetails(targetId, targetName);
    }
}

// История операций с целью
async function loadTargetTransactions(targetId) {
    const list = document.getElementById('target-history-list');
    const targetName = document.querySelector('h1').innerText; 

    try {
        const res = await fetch(`${API_URL}/TargetTransactions/${targetId}`);
        const txs = await res.json();

        if (txs.length === 0) {
            list.innerHTML = '<div style="text-align:center; padding:30px; color:#bbb;">История пуста</div>';
            return;
        }

        list.innerHTML = txs.map(t => {
            const isDeposit = t.type === 1;
            const amountClass = isDeposit ? 'tr-amount-pos' : 'tr-amount-neg';

            return `
                <div class="tr-item" style="border-left: 4px solid ${isDeposit ? '#28a745' : '#dc3545'};">
                    <button class="btn-delete-small" 
                        onclick="deleteTargetTransaction('${t.id}', '${targetId}', '${targetName}')" 
                        title="Удалить запись">&times;</button>
                    
                    <div class="tr-info">
                        <strong>${isDeposit ? 'Зачисление' : 'Снятие'}</strong>
                        ${t.description ? `<span class="tr-desc">${t.description}</span>` : ''}
                        <small style="color: #bcc0c4;">${new Date(t.createdTime).toLocaleDateString()}</small>
                    </div>
                    <span class="${amountClass}" style="margin-right: 25px;">
                        ${isDeposit ? '+' : '-'}${t.value.toLocaleString()} ₽
                    </span>
                </div>
            `;
        }).join('');
    } catch (e) {
        list.innerHTML = "Ошибка загрузки истории";
    }
}

// Общий капитал в левом сайдбаре
async function loadTotalBalance() {
    try {
        const res = await fetch(`${API_URL}/Accounts`);
        const accounts = await res.json();

        const totalSum = accounts.reduce((sum, acc) => sum + (acc.total || 0), 0);

        const container = document.getElementById('total-balance-container');
        container.innerHTML = `
            <div class="card total-balance-card" onclick="showTotalDashboard()">
                <div style="font-size: 0.8rem; opacity: 0.8; text-transform: uppercase; letter-spacing: 1px;">Общий капитал</div>
                <div style="font-size: 1.5rem; font-weight: 800; margin-top: 5px;">
                    ${totalSum.toLocaleString()} ₽
                </div>
            </div>
        `;
    } catch (e) {
        console.error("Ошибка загрузки общего баланса:", e);
    }
}

let currentFilterMonth = new Date().getMonth();
let currentFilterYear = new Date().getFullYear();
let currentFilterCategory = "all";
let currentFilterType = 0;
// Детализация общего капитала
async function showTotalDashboard() {
    const area = document.getElementById('details-area');
    area.innerHTML = '<div class="text-center mt-5">Анализ данных...</div>';

    try {
        let catUrl = `${API_URL}/Categories`;
        if (currentFilterType !== 0) catUrl += `?type=${currentFilterType}`;

        const [accRes, txRes, catRes] = await Promise.all([
            fetch(`${API_URL}/Accounts`),
            fetch(`${API_URL}/Transactions?page=1&limit=500`),
            fetch(catUrl)
        ]);

        const accounts = await accRes.json();
        const allTxs = await txRes.json();
        const categories = await catRes.json();

        const totalSum = accounts.reduce((sum, acc) => sum + (acc.total || 0), 0);
        const colors = ['#0084ff', '#28a745', '#ffc107', '#17a2b8', '#6610f2'];

        const externalTxs = allTxs.filter(t => t.relatedTransactionId === null);

        const periodTxs = externalTxs.filter(t => {
            const d = new Date(t.createdTime);
            const m = currentFilterMonth === -1 || d.getMonth() === currentFilterMonth;
            const y = d.getFullYear() === currentFilterYear;
            return m && y;
        });
        const incomeSum = periodTxs.filter(t => t.type === 1).reduce((sum, t) => sum + t.value, 0);
        const expenseSum = periodTxs.filter(t => t.type === 2).reduce((sum, t) => sum + t.value, 0);

        const filteredTxs = periodTxs.filter(t => {
            const tMatch = currentFilterType === 0 || t.type === currentFilterType;
            const cMatch = currentFilterCategory === "all" || t.categoryName === currentFilterCategory;
            return tMatch && cMatch;
        });

        area.innerHTML = `
            <h1 style="margin-bottom: 20px !important;">Общий обзор</h1>
            
            <div class="form-card" style="margin-bottom: 25px; padding: 20px; border-radius: 15px;">
                <h5 class="mb-3" style="font-weight: bold; color: #2d3748;">Распределение средств</h5>
                <div class="chart-container" style="height: 40px; display: flex; border-radius: 10px; overflow: hidden; background: #f0f2f5; margin-bottom: 20px;">
                    ${accounts.map((acc, i) => {
            const share = totalSum > 0 ? (acc.total / totalSum) * 100 : 0;
            if (share <= 0) return '';
            return `<div class="chart-segment" style="width:${share}%; background:${colors[i % colors.length]}; display:flex; align-items:center; justify-content:center; color:white; font-size:0.75rem; font-weight:bold; overflow:hidden;">${share > 7 ? Math.round(share) + '%' : ''}</div>`;
        }).join('')}
                </div>
                <div style="display: flex; flex-wrap: wrap; gap: 15px;">
                    ${accounts.filter(a => a.total > 0).map((acc, i) => `
                        <div style="display: flex; align-items: center; gap: 8px; font-size: 0.85rem;">
                            <div style="width: 12px; height: 12px; border-radius: 3px; background: ${colors[i % colors.length]};"></div>
                            <span style="color: #65676b;">${acc.name}:</span>
                            <span style="font-weight: bold;">${acc.total.toLocaleString()} ₽</span>
                        </div>`).join('')}
                </div>
            </div>

            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin-bottom: 30px;">
                <div class="form-card" onclick="toggleTypeFilter(1)" 
                     style="border-left: 5px solid #28a745; cursor: pointer; border: ${currentFilterType === 1 ? '2px solid #28a745' : '1px solid #eee'}; background: ${currentFilterType === 1 ? '#f1fdf7' : 'white'}; padding: 15px; margin:0;">
                    <small style="color:#888; text-transform:uppercase; font-size:0.7rem;">Доходы ${currentFilterType === 1 ? '●' : ''}</small>
                    <div style="font-size: 1.3rem; font-weight: bold; color: #28a745;">+${incomeSum.toLocaleString()} ₽</div>
                </div>
                <div class="form-card" onclick="toggleTypeFilter(2)" 
                     style="border-left: 5px solid #dc3545; cursor: pointer; border: ${currentFilterType === 2 ? '2px solid #dc3545' : '1px solid #eee'}; background: ${currentFilterType === 2 ? '#fff5f5' : 'white'}; padding: 15px; margin:0;">
                    <small style="color:#888; text-transform:uppercase; font-size:0.7rem;">Расходы ${currentFilterType === 2 ? '●' : ''}</small>
                    <div style="font-size: 1.3rem; font-weight: bold; color: #dc3545;">-${expenseSum.toLocaleString()} ₽</div>
                </div>
            </div>

            <div style="display: flex !important; justify-content: space-between !important; align-items: center !important; margin-bottom: 15px !important; width: 100% !important; min-height: 40px !important;">

        <h4 style="margin: 0 !important; font-size: 1.1rem; display: flex !important; align-items: center !important; height: 100% !important;">
            ${currentFilterType === 0 ? 'Все операции' : currentFilterType === 1 ? 'Только доходы' : 'Только расходы'}
        </h4>

        <div style="display: flex !important; gap: 10px !important; align-items: center !important;">
            
            <select class="select-inline" onchange="changeGlobalFilter(this.value, null, null)" 
                style="height: 38px !important; border-radius: 8px !important; margin: 0 !important; box-sizing: border-box !important;">
                <option value="-1" ${currentFilterMonth === -1 ? 'selected' : ''}>Все месяцы</option>
                ${['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'].map((m, i) => `<option value="${i}" ${i === currentFilterMonth ? 'selected' : ''}>${m}</option>`).join('')}
            </select>

            <select class="select-inline" onchange="changeGlobalFilter(null, this.value, null)" 
                style="height: 38px !important; border-radius: 8px !important; margin: 0 !important; box-sizing: border-box !important;">
                <option value="2025" ${currentFilterYear === 2025 ? 'selected' : ''}>2025</option>
                <option value="2026" ${currentFilterYear === 2026 ? 'selected' : ''}>2026</option>
            </select>

            <select class="select-inline" onchange="changeGlobalFilter(null, null, this.value)" 
                style="height: 38px !important; border-radius: 8px !important; width: 150px !important; margin: 0 !important; box-sizing: border-box !important;">
                <option value="all" ${currentFilterCategory === "all" ? 'selected' : ''}>Все категории</option>
                ${categories.map(c => `<option value="${c.name || c.Name}" ${currentFilterCategory === (c.name || c.Name) ? 'selected' : ''}>${c.name || c.Name}</option>`).join('')}
            </select>
        </div>
    </div>

            <div id="global-transactions-list">
                ${filteredTxs.length > 0 ? filteredTxs.map(t => `
                    <div class="tr-item">
                        <div class="tr-info">
                            <strong>${t.categoryName || 'Без категории'}</strong>
                            <div style="font-size: 0.75rem; color: #0084ff;">${t.accountName}</div>
                            <small>${new Date(t.createdTime).toLocaleDateString()}</small>
                        </div>
                        <span class="${t.type === 2 ? 'tr-amount-neg' : 'tr-amount-pos'}">
                            ${t.type === 2 ? '-' : '+'}${t.value.toLocaleString()} ₽
                        </span>
                    </div>`).join('') : '<p class="text-center text-muted p-5">Транзакций не найдено</p>'}
            </div>
        `;
    } catch (e) { console.error("Ошибка дашборда:", e); area.innerHTML = "<h2>Ошибка загрузки данных</h2>"; }
}

// Выпадающий список для фильтраций
function changeGlobalFilter(month, year, category) {
    if (month !== null) currentFilterMonth = parseInt(month);
    if (year !== null) currentFilterYear = parseInt(year);
    if (category !== null) currentFilterCategory = category;
    showTotalDashboard();
}

// Для кликов по карточкам доходы / расходы
function toggleTypeFilter(type) {
    currentFilterType = (currentFilterType === type) ? 0 : type;
    currentFilterCategory = "all";
    showTotalDashboard();
}


loadTotalBalance();
loadTargets();
loadAccounts();