// Кастомный мультиселект с чекбоксами
class MultiSelect {
    constructor(element) {
        this.element = element;
        this.options = Array.from(element.options);
        this.selectedValues = [];
        this.init();
    }

    init() {
        // Скрываем оригинальный select
        this.element.style.display = 'none';
        
        // Создаем кастомный dropdown
        this.wrapper = document.createElement('div');
        this.wrapper.className = 'multiselect-wrapper';
        
        // Поле для отображения выбранных элементов
        this.display = document.createElement('div');
        this.display.className = 'multiselect-display';
        this.display.innerHTML = '<span class="placeholder">Выберите...</span>';
        
        // Стрелка dropdown
        const arrow = document.createElement('span');
        arrow.className = 'multiselect-arrow';
        arrow.innerHTML = '▼';
        this.display.appendChild(arrow);
        
        // Выпадающий список
        this.dropdown = document.createElement('div');
        this.dropdown.className = 'multiselect-dropdown';
        
        // Заполняем dropdown опциями с чекбоксами
        this.populateDropdown();
        
        // Собираем компонент
        this.wrapper.appendChild(this.display);
        this.wrapper.appendChild(this.dropdown);
        this.element.parentNode.insertBefore(this.wrapper, this.element.nextSibling);
        
        // События
        this.display.addEventListener('click', () => this.toggleDropdown());
        document.addEventListener('click', (e) => this.handleOutsideClick(e));
        document.addEventListener('keydown', (e) => this.handleKeyDown(e));
        
        // Инициализируем выбранные значения
        this.updateFromSelect();
    }
    populateDropdown() {
        // Поле поиска
        const searchWrapper = document.createElement('div');
        searchWrapper.className = 'multiselect-search';
        searchWrapper.innerHTML = `
            <input type="text" placeholder="Поиск..." class="multiselect-search-input">
        `;
        this.searchInput = searchWrapper.querySelector('input');
        this.searchInput.addEventListener('input', (e) => this.filterOptions(e.target.value));
        this.searchInput.addEventListener('click', (e) => e.stopPropagation());
        this.dropdown.appendChild(searchWrapper);
        
        // Опция "Все"
        if (this.options.length > 0 && this.options[0].value === '') {
            const allOption = document.createElement('div');
            allOption.className = 'multiselect-option all-option';
            allOption.innerHTML = `
                <label>
                    <input type="checkbox" class="select-all-checkbox">
                    <span>Выбрать все</span>
                </label>
            `;
            const checkbox = allOption.querySelector('input');
            checkbox.addEventListener('change', () => this.toggleAll(checkbox.checked));
            this.dropdown.appendChild(allOption);
            
            // Разделитель
            const separator = document.createElement('div');
            separator.className = 'multiselect-separator';
            this.dropdown.appendChild(separator);
        }
        
        // Контейнер для опций
        this.optionsContainer = document.createElement('div');
        this.optionsContainer.className = 'multiselect-options-container';
        
        // Остальные опции
        this.options.forEach((option, index) => {
            if (option.value === '') return; // Пропускаем пустую опцию
            
            const optionDiv = document.createElement('div');
            optionDiv.className = 'multiselect-option';
            optionDiv.dataset.text = option.text.toLowerCase();
            optionDiv.innerHTML = `
                <label>
                    <input type="checkbox" value="${option.value}" data-index="${index}">
                    <span>${option.text}</span>
                </label>
            `;
            
            const checkbox = optionDiv.querySelector('input');
            checkbox.addEventListener('change', () => this.toggleOption(option.value, checkbox.checked));
            
            this.optionsContainer.appendChild(optionDiv);
        });
        
        this.dropdown.appendChild(this.optionsContainer);
    }
    toggleDropdown() {
        this.dropdown.classList.toggle('show');
        this.wrapper.classList.toggle('active');
        if (this.dropdown.classList.contains('show') && this.searchInput) {
            this.searchInput.focus();
            this.searchInput.value = '';
            this.filterOptions('');
        }
    }
    
    hideDropdown() {
        this.dropdown.classList.remove('show');
        this.wrapper.classList.remove('active');
    }
    
    filterOptions(searchTerm) {
        const term = searchTerm.toLowerCase();
        const options = this.optionsContainer.querySelectorAll('.multiselect-option');
        
        options.forEach(option => {
            const text = option.dataset.text;
            if (text.includes(term)) {
                option.style.display = '';
            } else {
                option.style.display = 'none';
            }
        });
    }
    
    handleOutsideClick(e) {
        if (!this.wrapper.contains(e.target)) {
            this.hideDropdown();
        }
    }
    
    handleKeyDown(e) {
        if (e.key === 'Escape' && this.dropdown.classList.contains('show')) {
            this.hideDropdown();
        }
    }
    
    toggleOption(value, checked) {
        if (checked) {
            if (!this.selectedValues.includes(value)) {
                this.selectedValues.push(value);
            }
        } else {
            this.selectedValues = this.selectedValues.filter(v => v !== value);
        }
        
        this.updateDisplay();
        this.updateOriginalSelect();
        this.updateSelectAllCheckbox();
    }
    
    toggleAll(checked) {
        const checkboxes = this.dropdown.querySelectorAll('input[type="checkbox"]:not(.select-all-checkbox)');
        checkboxes.forEach(cb => {
            cb.checked = checked;
            const value = cb.value;
            if (checked && !this.selectedValues.includes(value)) {
                this.selectedValues.push(value);
            }
        });
        
        if (!checked) {
            this.selectedValues = [];
        }
        
        this.updateDisplay();
        this.updateOriginalSelect();
    }
    updateDisplay() {
        if (this.selectedValues.length === 0) {
            this.display.innerHTML = '<span class="placeholder">Выберите...</span>';
        } else {
            const selectedTexts = this.selectedValues.map(value => {
                const option = this.options.find(o => o.value === value);
                return option ? option.text : '';
            }).filter(t => t);
            
            if (selectedTexts.length <= 3) {
                this.display.innerHTML = selectedTexts.join(', ');
            } else {
                this.display.innerHTML = `Выбрано: ${selectedTexts.length}`;
            }
        }
        
        // Добавляем стрелку обратно
        const arrow = document.createElement('span');
        arrow.className = 'multiselect-arrow';
        arrow.innerHTML = '▼';
        this.display.appendChild(arrow);
    }
    
    updateOriginalSelect() {
        // Обновляем оригинальный select
        this.options.forEach(option => {
            option.selected = this.selectedValues.includes(option.value);
        });
        
        // Триггерим событие change
        const event = new Event('change', { bubbles: true });
        this.element.dispatchEvent(event);
    }
    
    updateSelectAllCheckbox() {
        const selectAllCheckbox = this.dropdown.querySelector('.select-all-checkbox');
        if (selectAllCheckbox) {
            const totalOptions = this.options.filter(o => o.value !== '').length;
            selectAllCheckbox.checked = this.selectedValues.length === totalOptions;
            selectAllCheckbox.indeterminate = this.selectedValues.length > 0 && this.selectedValues.length < totalOptions;
        }
    }
    
    updateFromSelect() {
        // Синхронизация с оригинальным select при инициализации
        this.selectedValues = [];
        this.options.forEach(option => {
            if (option.selected && option.value !== '') {
                this.selectedValues.push(option.value);
            }
        });
        
        // Обновляем чекбоксы
        const checkboxes = this.dropdown.querySelectorAll('input[type="checkbox"]:not(.select-all-checkbox)');
        checkboxes.forEach(cb => {
            cb.checked = this.selectedValues.includes(cb.value);
        });
        
        this.updateDisplay();
        this.updateSelectAllCheckbox();
    }
}

// Инициализация всех мультиселектов на странице
document.addEventListener('DOMContentLoaded', function() {
    const multiSelects = document.querySelectorAll('select[multiple]');
    multiSelects.forEach(select => {
        new MultiSelect(select);
    });
});