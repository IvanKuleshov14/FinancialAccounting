const API_URL = 'https://localhost:7249';

// Загрузка счетов в сайдбар
async function loadAccounts() {
    const res = await fetch(`${API_URL}/Accounts`);
    const data = await res.json();
    const list = document.getElementById('accounts-list');

    list.innerHTML = data.map(acc => {
        // Используем TargetProgress напрямую из твоего DTO
        const progressValue = acc.targetProgress || 0;
        const barWidth = Math.min(progressValue, 100); // Чтобы бар не улетел за 100%

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

// Показ истории транзакций
async function showAccount(id, name) {
    const area = document.getElementById('details-area');
    area.innerHTML = `
    <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:30px;">
        <div id="account-title-container">
            <h1 style="display:inline-block;">${name}</h1>
            <button class="btn-edit" onclick="enableEditAccount('${id}', '${name}')">✎</button>
            <div id="account-target-badge"></div> <!-- Место для названия цели -->
        </div>
        <div style="display:flex; gap:10px;">
            <button class="btn-submit" style="background:#6c757d; width:auto; padding:10px 20px;" onclick="showAccountTargetForm('${id}', '${name}')">🎯 Цель</button>
            <button class="btn-submit" style="background:#6c757d; width:auto; padding:10px 20px;" onclick="showTransferForm('${id}', '${name}')">⇄ Перевод</button>
            <button class="btn-submit" style="width:auto; padding:10px 20px;" onclick="showForm('${id}', '${name}')">+ Операция</button>
            <div class="dropdown">
                 <button class="btn-more" onclick="toggleAccountMenu()">⋮</button>
                 <div id="account-dropdown" class="dropdown-content">
                    <button onclick="deleteAccount('${id}')">🗑 Удалить счет</button>
                 </div>
            </div>
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
let allCategories = []; // Глобальная переменная для хранения категорий

async function showForm(accountId, accountName) {
    const area = document.getElementById('details-area');

    // 1. Загружаем категории, если еще не загрузили
    try {
        const catRes = await fetch(`${API_URL}/Categories`);
    } catch (e) {
        console.error("Ошибка загрузки категорий", e);
    }

    area.innerHTML = `
        <div class="form-card">
            <h2 style="text-align:center; margin-bottom:20px;">Новая транзакция</h2>
            <h4 style="text-align:center; color:#0084ff; margin-bottom:20px;">${accountName}</h4>
            
            <!-- Переключатель типа с вызовом фильтрации -->
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
    <!-- Выпадающий список забирает всё свободное место -->
    <select id="category-select" style="flex-grow: 1; height: 45px; margin: 0;">
        <!-- Загрузится динамически -->
    </select>

    <!-- Кнопка РЕДАКТИРОВАНИЯ (с отступом справа) -->
    <button type="button" class="btn-inline-add"
            onclick="showEditCategoryForm('${accountId}', '${accountName}')" 
            style="
                background: #f0f2f5; 
                color: #65676b; 
                border: 1px solid #ddd; 
                margin-right: 12px !important; /* ОТСТУП ЗДЕСЬ */
                width: 45px; height: 45px; flex-shrink: 0;
            " title="Редактировать категорию">✎</button>

    <!-- Кнопка ДОБАВЛЕНИЯ новой (стоит чуть поодаль) -->
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
        // Делаем запрос к твоему методу, передавая тип (1 или 2)
        // Проверь, как именно в контроллере прописан маршрут (query string или route)
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
async function saveTransaction(accountId, accountName) { // Добавили accountName
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

// Функция удаления счета
async function deleteAccount(id) {
    if (!confirm("Вы уверены, что хотите удалить этот счет со всеми транзакциями? Это действие нельзя отменить.")) return;

    try {
        const res = await fetch(`${API_URL}/Accounts/${id}`, {
            method: 'DELETE'
        });

        if (res.ok) {
            await loadAccounts(); // Обновляем сайдбар
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
        // Если перенаправление на главную после удаления
        location.reload();
    }
}

// Показать форму создания счета
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
        // Используем случайный валидный GUID вместо нулей
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
            // Если твой контроллер возвращает Created/Ok с объектом
            try {
                const createdAccount = await res.json();
                await loadAccounts();
                showAccount(createdAccount.id, createdAccount.name);
            } catch {
                // Если контроллер вернул успех, но без тела (пустой Ok)
                await loadAccounts();
                document.getElementById('details-area').innerHTML = "<h2>Счет создан</h2>";
            }
        } else {
            const err = await res.text();
            alert("Сервер не принял UserId: " + err);
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
    }
}

// Форма для создания привязанной цели
async function showAccountTargetForm(accountId, accountName) {
    const area = document.getElementById('details-area');
    area.innerHTML = '<div class="text-center mt-5">Загрузка данных цели...</div>';

    try {
        const res = await fetch(`${API_URL}/Accounts/${accountId}`);
        const acc = await res.json();

        // Если TargetName пустой — рисуем форму СОЗДАНИЯ
        if (!acc.targetName) {
            renderCreateTargetForm(accountId, accountName);
        } else {
            // Если цель есть — рисуем ДЕТАЛИЗАЦИЮ
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
            
            <!-- КНОПКА УДАЛЕНИЯ: Жесткое позиционирование через inline-style -->
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
            await loadAccounts(); // Обновит прогресс-бар в сайдбаре
            showAccount(accountId, accountName);
        } else {
            alert("Ошибка при установке цели");
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
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
            await loadAccounts(); // Чтобы убрать бар из сайдбара
            showAccount(accountId, accountName); // Возвращаемся в детали счета
        } else {
            alert("Ошибка при удалении цели с сервера");
        }
    } catch (e) {
        console.error(e);
        // Fallback для CORS
        await loadAccounts();
        showAccount(accountId, accountName);
    }
}

function showCreateCategoryForm(accountId, accountName) {
    const area = document.getElementById('details-area');
    // Определяем текущий тип из радио-кнопок (1 или 2)
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

async function submitCreateCategory(accountId, accountName, type) {
    const name = document.getElementById('new-cat-name').value;
    if (!name) return alert("Введите название");

    const payload = {
        name: name,
        type: type // 1 для дохода, 2 для расхода
    };

    try {
        const res = await fetch(`${API_URL}/Categories`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (res.ok) {
            // Возвращаемся в форму транзакции
            showForm(accountId, accountName);
            // После отрисовки формы нужно принудительно обновить список категорий для текущего типа
            setTimeout(() => updateCategoryList(type), 100);
        } else {
            alert("Ошибка при создании категории");
        }
    } catch (e) {
        console.error(e);
        showForm(accountId, accountName);
    }
}

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
            <button onclick="showAccount('${accountId}', '${accountName}')" style="background:none; border:none; color:#888; width:100%; margin-top:10px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

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
            await loadAccounts(); // Чтобы обновился сайдбар
            showAccount(accountId, accountName); // Возвращаемся в счет
        } else {
            alert("Ошибка при обновлении цели");
        }
    } catch (e) {
        console.error(e);
        await loadAccounts();
        showAccount(accountId, accountName);
    }
}

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
                
                <!-- Кнопка удаления внутри формы -->
                <button class="btn-submit" style="background: #fff5f5; color: #dc3545; border: 1px solid #ffc1c1; width: 60px;" 
                        onclick="deleteCurrentCategoryConfirm('${categoryId}', '${accountId}', '${accountName}')" title="Удалить">🗑</button>
            </div>

            <button onclick="showForm('${accountId}', '${accountName}')" style="background:none; border:none; color:#888; width:100%; margin-top:15px; cursor:pointer;">Отмена</button>
        </div>
    `;
}

// 1. Метод PUT для изменения имени
async function submitUpdateCategory(categoryId, accountId, accountName) {
    const newName = document.getElementById('edit-cat-name').value;
    if (!newName) return alert("Введите название");

    try {
        const res = await fetch(`${API_URL}/Categories/${categoryId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: newName }) // Твой UpdateCategoryDto
        });

        if (res.ok) {
            showForm(accountId, accountName);
        }
    } catch (e) { console.error(e); showForm(accountId, accountName); }
}

// 2. Метод DELETE для Soft Delete
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

// Запуск
loadAccounts();