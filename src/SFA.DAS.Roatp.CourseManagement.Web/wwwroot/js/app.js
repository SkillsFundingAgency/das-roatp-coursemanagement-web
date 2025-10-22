
$(function () {

  // Autocomplete on standards dropdowns  

  var selectElements = $('.app-autocomplete')
    selectElements.each(function () {
        var form = $(this).closest('form');
  
        // Hide the original select field from screen readers 
        const hiddenSelect = document.getElementById(this.id);
        hiddenSelect.setAttribute('aria-hidden', 'true');
        hiddenSelect.setAttribute('tabindex', '-1');
        hiddenSelect.setAttribute('title', 'Hidden select field');
        
        accessibleAutocomplete.enhanceSelectElement({
            selectElement: this,
            minLength: 3,
            autoselect: false,
            defaultValue: '',
            showAllValues: true,
            displayMenu: 'overlay',
            dropdownArrow: function () {
                return '<svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" clip-rule="evenodd" d="M13.4271 15.4271C12.0372 16.4175 10.3367 17 8.5 17C3.80558 17 0 13.1944 0 8.5C0 3.80558 3.80558 0 8.5 0C13.1944 0 17 3.80558 17 8.5C17 10.3367 16.4175 12.0372 15.4271 13.4271L21.0119 19.0119C21.5621 19.5621 21.5575 20.4425 21.0117 20.9883L20.9883 21.0117C20.4439 21.5561 19.5576 21.5576 19.0119 21.0119L13.4271 15.4271ZM8.5 15C12.0899 15 15 12.0899 15 8.5C15 4.91015 12.0899 2 8.5 2C4.91015 2 2 4.91015 2 8.5C2 12.0899 4.91015 15 8.5 15Z" fill="#b1b4b6"/></svg>';
            },
            placeholder: $(this).data('placeholder') || '',
            onConfirm: function (opt) {
                var txtInput = document.querySelector('#' + this.id);
                var searchString = opt || txtInput.value;
                var requestedOption = [].filter.call(this.selectElement.options,
                    function (option) {
                        return (option.textContent || option.innerText) === searchString
                    }
                )[0];
                if (requestedOption) {
                    requestedOption.selected = true;
                } else {
                    this.selectElement.selectedIndex = 0;
                }
            }
        });
        form.on('submit', function() {
            $('.autocomplete__input').each(function() {
                var that = $(this);
                if (that.val().length === 0) {
                    var fieldId = that.attr('id'),
                    selectField = $('#' + fieldId + '-select');
                    selectField[0].selectedIndex = 0;
                }
            });
        });
    })

function AutoComplete(selectField) {
    this.selectElement = selectField
}

AutoComplete.prototype.init = function () {
    this.selectElement.setAttribute('aria-hidden', 'true');
    this.selectElement.setAttribute('tabindex', '-1');
    this.selectElement.setAttribute('title', 'Hidden input field');
    this.autoComplete()
}

AutoComplete.prototype.getSuggestions = function (query, updateResults) {
    let results = [];
    let apiUrl = "/locations?query=" + query
    let xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            let jsonResponse = JSON.parse(xhr.responseText);
            results = jsonResponse.map(function (result) {
                return result
            });
            updateResults(results);
        }
    }
    xhr.open("GET", apiUrl, true);
    xhr.send();
}

AutoComplete.prototype.onConfirm = function (option) {
    // Populate form fields with selected option
    document.getElementById("AddressLine1").value = option.addressLine1
    document.getElementById("AddressLine2").value = option.addressLine2
    document.getElementById("Town").value = option.town
    document.getElementById("County").value = option.county
    document.getElementById("Postcode").value = option.postcode
    document.getElementById("Longitude").value = option.longitude
    document.getElementById("Latitude").value = option.latitude
}

function inputValueTemplate(result) {
    return result && [result.addressLine1, result.addressLine2, result.town, result.postcode].filter(element => element).join(', ')
}

function suggestionTemplate(result) {
    return result && [result.addressLine1, result.addressLine2, result.town, result.postcode].filter(element => element).join(', ')
}

AutoComplete.prototype.autoComplete = function () {
    let that = this
    accessibleAutocomplete.enhanceSelectElement({
        selectElement: that.selectElement,
        minLength: 3,
        autoselect: false,
        defaultValue: '',
        displayMenu: 'overlay',
        placeholder: '',
        source: that.getSuggestions,
        showAllValues: false,
        confirmOnBlur: false,
        onConfirm: that.onConfirm,
        templates: {
            inputValue: inputValueTemplate,
            suggestion: suggestionTemplate
        }
    });
}

function nodeListForEach(nodes, callback) {
    if (window.NodeList.prototype.forEach) {
        return nodes.forEach(callback)
    }
    for (let i = 0; i < nodes.length; i++) {
        callback.call(window, nodes[i], i, nodes);
    }
}

let autoCompletes = document.querySelectorAll('[data-module="autoComplete"]')

nodeListForEach(autoCompletes, function (autoComplete) {
    new AutoComplete(autoComplete).init()
})


});

/*select standards for added provider contact*/

const selectAllStandardsCheckbox = document.getElementById('providerCourseStandardsCheckAll');
const providerContactStandardsCheckboxes = document.getElementsByClassName('providerContactStandards');

selectAllStandardsCheckbox.addEventListener('change', (event) => {
    selectAllProviderContactStandards();
});


for (const item of providerContactStandardsCheckboxes) {
    item.addEventListener('change', (event) => {
        checkSelectAll();
    });
}

checkSelectAll();

function selectAllProviderContactStandards() {
    for (const item of providerContactStandardsCheckboxes) {
        item.checked = selectAllStandardsCheckbox.checked;
    }
}

function checkSelectAll() {
    let allChecked = true;
    for (const item of providerContactStandardsCheckboxes) {
        if (!item.checked) {
            allChecked = false;
        }
    }

    selectAllStandardsCheckbox.checked = allChecked;
}