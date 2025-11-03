window.interop = {
    escPressListener: null,

    /**
     * **getElement**  
     * Retorna uma promessa que resolve se o seletor de CSS referente ao `elementRef` for encontrado, rejeita caso contrário.
     * @param {string} elementRef - Referência do elemento (seletor CSS).
     * @returns {Promise<boolean>} - Resolve se o elemento for encontrado, rejeita caso contrário.
     */
    getElement: function (elementRef) {
        return new Promise((resolve, reject) => {
            try {
                const selector = document.querySelector(elementRef);
                if (selector) {
                    resolve(true);
                } else {
                    reject(false);
                }
            } catch (error) {
                reject(error);
            }
        });
    },

    /**
     * **isEleExist**  
     * Verifica se o elemento referenciado por `elementRef` existe no DOM.
     * @param {string} elementRef - Referência do elemento (seletor CSS).
     * @returns {boolean} - Retorna `true` se o elemento existir, `false` caso contrário.
     */
    isEleExist: function (elementRef) {
        // Function to get the CSS selector for the given element reference
        const selector = document.querySelector(elementRef);
        if (selector) {
            return true;
        } else {
            return false;
        }
    },

    /**
     * **getBoundry**  
     * Obtém as coordenadas e dimensões do elemento referenciado.
     * @param {HTMLElement} elementRef - Referência ao elemento do DOM.
     * @returns {DOMRect} - Retorna o objeto `DOMRect` contendo a posição e tamanho do elemento.
     */
    getBoundry: function (elementRef) {
        const rect = elementRef?.getBoundingClientRect();
        return rect;
    },

    /**
     * **inner**  
     * Obtém as dimensões da janela (largura ou altura).
     * @param {string} arg - String que define se deve retornar a largura ou altura (`innerWidth` ou `innerHeight`).
     * @returns {number} - Largura ou altura da janela, ou `0` se o argumento não for válido.
     */
    inner: function (arg) {
        if (arg == "innerWidth") {
            return window.innerWidth ?? 992;
        }
        if (arg == "innerHeight") {
            return window.innerHeight ?? 992;
        }
        return 0
    },

    /**
     * **MenuNavElement**  
     * Obtém a largura total (scrollWidth) e a margem inicial de um elemento.
     * @param {string} elementRef - Seletor CSS do elemento.
     * @returns {Promise<object>} - Promessa que resolve com um objeto contendo `scrollWidth` e `marginInlineStart`.
     */
    MenuNavElement: function (elementRef) {
        // Function to get the width of the given element reference
        return new Promise((resolve, reject) => {
            try {
                const element = document.querySelector(elementRef);
                if (element) {
                    const scrollWidth = element.scrollWidth; // Get the scrollWidth of the element
                    const marginInlineStart = Math.ceil(
                        Number(
                            window.getComputedStyle(element).marginInlineStart.split("px")[0]
                        )
                    ); // Get the scrollWidth of the element
                    resolve({ scrollWidth, marginInlineStart }); // Return both element and width
                } else {
                    reject("Element not found");
                }
            } catch (error) {
                reject(error);
            }
        });
    },

    /**
     * **MenuNavmarginInlineStart**  
     * Define a margem inicial (`marginInlineStart`) de um elemento.
     * @param {string} selector - Seletor CSS do elemento.
     * @param {string} value - Valor para a margem inicial.
     * @returns {Promise<HTMLElement>} - Promessa que resolve com o elemento atualizado.
     */
    MenuNavmarginInlineStart: function (selector, value) {
        // Function to get the width of the given element reference
        return new Promise((resolve, reject) => {
            try {
                const element = document.querySelector(selector);
                if (element) {
                    element.style.marginInlineStart = value;
                    resolve(element); // Return both element and width
                } else {
                    reject("Element not found");
                }
            } catch (error) {
                reject(error);
            }
        });
    },

    /**
     * **mainSidebarOffset**  
     * Obtém a largura de um elemento específico (como uma barra lateral).
     * @param {string} elementRef - Seletor CSS do elemento.
     * @returns {Promise<number>} - Promessa que resolve com a largura (`offsetWidth`) do elemento.
     */
    mainSidebarOffset: function (elementRef) {
        // Function to get the width of the given element reference
        return new Promise((resolve, reject) => {
            try {
                const element = document.querySelector(elementRef);
                if (element) {
                    const mainSidebarOffset = element.offsetWidth; // Get the scrollWidth of the element
                    resolve(mainSidebarOffset); // Return both element and width
                } else {
                    reject("Element not found");
                }
            } catch (error) {
                reject(error);
            }
        });
    },

    /**
     * **addClass**  
     * Adiciona uma classe CSS a um elemento.
     * @param {string} elementRef - Seletor CSS do elemento.
     * @param {string} className - Classe a ser adicionada.
     */
    addClass: function (elementRef, className) {
        const element = document.querySelector(elementRef);
        if (element) {
            element.classList.add(className);
        }
    },

/**
     * **removeClass**  
     * Remove uma classe CSS de um elemento.
     * @param {string} elementRef - Seletor CSS do elemento.
     * @param {string} className - Classe a ser removida.
     */
    removeClass: function (elementRef, className) {
        const element = document.querySelector(elementRef);
        if (element) {
            element.classList.remove(className);
        }
    },

    /**
     * **addClassToHtml**  
     * Adiciona uma classe ao elemento `<html>`.
     * @param {string} className - Classe a ser adicionada.
     */
    addClassToHtml: (className) => {
        document.documentElement.classList.add(className);
    },

    /**
     * **setclearCssVariables**  
     * Limpa todas as variáveis de CSS do elemento `<html>`.
     */
    setclearCssVariables: function () {
        document.documentElement.style = "";
    },

    /**
     * **setCssVariable**  
     * Define uma variável de CSS no elemento `<html>`.
     * @param {string} variableName - Nome da variável CSS.
     * @param {string} value - Valor da variável.
     */
    setCssVariable: function (variableName, value) {
        document.documentElement.style.setProperty(variableName, value);
    },

    /**
     * **removeCssVariable**  
     * Remove uma variável de CSS do elemento `<html>`.
     * @param {string} variableName - Nome da variável CSS.
     */
    removeCssVariable: function (variableName, value) {
        document.documentElement.style.removeProperty(variableName, value);
    },

    /**
     * **setCustomCssVariable**  
     * Define uma variável CSS personalizada para um elemento específico.
     * @param {string} element - Seletor do elemento onde a variável será aplicada.
     * @param {string} variableName - Nome da variável CSS a ser definida.
     * @param {string} value - Valor da variável CSS.
     */
    setCustomCssVariable: function (element, variableName, value) {
        let ele = document.querySelector(element);
        if (ele) {
            ele.style.setProperty(variableName, value);
        }
    },

    /**
     * **removeClassFromHtml**  
     * Remove uma classe específica do elemento `<html>`.
     * @param {string} className - Nome da classe a ser removida.
     */
    removeClassFromHtml: (className) => {
        document.documentElement.classList.remove(className);
    },

    /**
     * **getAttributeToHtml**  
     * Obtém o valor de um atributo do elemento `<html>`.
     * @param {string} attributeName - Nome do atributo.
     * @returns {string | null} - Valor do atributo ou `null` se não existir.
     */
    getAttributeToHtml: (attributeName) => {
        return document.documentElement.getAttribute(attributeName);
    },

    /**
     * **addAttributeToHtml**  
     * Adiciona um atributo ao elemento `<html>`.
     * @param {string} attributeName - Nome do atributo a ser adicionado.
     * @param {string} attributeValue - Valor do atributo.
     */
    addAttributeToHtml: (attributeName, attributeValue) => {
        document.documentElement.setAttribute(attributeName, attributeValue);
    },

    /**
     * **removeAttributeFromHtml**  
     * Remove um atributo do elemento `<html>`.
     * @param {string} attributeName - Nome do atributo a ser removido.
     */
    removeAttributeFromHtml: (attributeName) => {
        document.documentElement.removeAttribute(attributeName);
    },

    /**
     * **getAttribute**  
     * Obtém o valor de um atributo de um elemento específico.
     * @param {string} elementRef - Seletor do elemento.
     * @param {string} attributeName - Nome do atributo.
     * @returns {Promise<string | null>} - Uma Promise que resolve com o valor do atributo ou rejeita com um erro.
     */
    getAttribute: function (elementRef, attributeName) {
        return new Promise((resolve, reject) => {
            try {
                const selector = document.querySelector(elementRef);
                if (selector) {
                    resolve(selector.getAttribute(attributeName));
                } else {
                    reject("Element not found");
                }
            } catch (error) {
                reject(error);
            }
        });
    },

    /**
     * **setAttribute**  
     * Define um atributo para um elemento específico.
     * @param {string} elementRef - Seletor do elemento.
     * @param {string} attributeName - Nome do atributo.
     * @param {string} attributeValue - Valor do atributo.
     * @returns {Promise<void>} - Uma Promise que resolve após definir o atributo ou rejeita com um erro.
     */
    setAttribute: function (elementRef, attributeName, attributeValue) {
        return new Promise((resolve, reject) => {
            try {
                const selector = document.querySelector(elementRef);
                if (selector) {
                    resolve(selector.setAttribute(attributeName, attributeValue));
                } else {
                    reject("Element not found");
                }
            } catch (error) {
                reject(error);
            }
        });
    },

    /**
     * **setLocalStorageItem**  
     * Define um item no localStorage.
     * @param {string} key - Chave do item a ser definido.
     * @param {string} value - Valor do item a ser definido.
     */
    setLocalStorageItem: function (key, value) {
        localStorage.setItem(key, value);
    },

    /**
     * **removeLocalStorageItem**  
     * Remove um item do localStorage.
     * @param {string} key - Chave do item a ser removido.
     */
    removeLocalStorageItem: function (key) {
        localStorage.removeItem(key);
    },

    /**
     * **getAllLocalStorageItem**  
     * Retorna todos os itens do localStorage.
     * @returns {Storage} - Objeto contendo todos os itens do localStorage.
     */
    getAllLocalStorageItem: function () {
        return localStorage;
    },

    /**
     * **getLocalStorageItem**  
     * Obtém o valor de um item do localStorage.
     * @param {string} key - Chave do item.
     * @returns {string | null} - Valor do item ou `null` se não existir.
     */
    getLocalStorageItem: function (key) {
        return localStorage.getItem(key);
    },

    /**
     * **clearAllLocalStorage**  
     * Limpa todos os itens do localStorage.
     */
    clearAllLocalStorage: function () {
        localStorage.clear();
    },

    /**
     * **directionChange**  
     * Verifica a direção do menu com base na localização do item e a largura da janela.  
     * @param {string} dataId - O `data-id` do elemento para verificação da direção.
     * @returns {boolean} - Retorna `true` se a direção do menu precisa ser ajustada, caso contrário, `false`.
     */
    directionChange: function (dataId) {
        let element = document.querySelector(`[data-id="${dataId}"]`);
        let html = document.documentElement;
        if (element) {
            const listItem = element.closest("li");
            if (listItem) {
                // Find the first sibling <ul> element
                const siblingUL = listItem.querySelector("ul");
                let outterUlWidth = 0;
                let listItemUL = listItem.closest("ul:not(.main-menu)");
                while (listItemUL) {
                    listItemUL = listItemUL.parentElement.closest("ul:not(.main-menu)");
                    if (listItemUL) {
                        outterUlWidth += listItemUL.clientWidth;
                    }
                }
                if (siblingUL) {
                    // You've found the sibling <ul> element
                    let siblingULRect = listItem.getBoundingClientRect();
                    if (html.getAttribute("dir") == "rtl") {
                        if (
                            siblingULRect.left - siblingULRect.width - outterUlWidth + 150 <
                            0 &&
                            outterUlWidth < window.innerWidth &&
                            outterUlWidth + siblingULRect.width + siblingULRect.width <
                            window.innerWidth
                        ) {
                            return true;
                        } else {
                            return false;
                        }
                    } else {
                        if (
                            outterUlWidth + siblingULRect.right + siblingULRect.width + 50 >
                            window.innerWidth &&
                            siblingULRect.right >= 0 &&
                            outterUlWidth + siblingULRect.width + siblingULRect.width <
                            window.innerWidth
                        ) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                }
            }
        }
        return false;
    },

    /**
     * **groupDirChange**  
     * Ajusta a direção dos itens de menu com base no layout atual (horizontal/vertical) e o espaço disponível.  
     * Retorna uma lista de itens que precisam de ajustes na posição (adicionados ou removidos).  
     * @returns {Object} - Um objeto contendo três listas: `added` (itens a serem ajustados), `removed` (itens removidos do ajuste), e `clearNavDropdown` (indicador se o dropdown deve ser limpo).
     */
    groupDirChange: function () {
        let elemList = {
            added: [],
            removed: [],
            clearNavDropdown: false,
        };
        if (
            document.querySelector("html").getAttribute("data-nav-layout") ===
            "horizontal" &&
            window.innerWidth > 992
        ) {
            let activeMenus = document.querySelectorAll(".slide.has-sub.open > ul");
            activeMenus.forEach((e) => {
                let target = e;
                let html = document.documentElement;

                const listItem = target.closest("li");
                // Get the position of the clicked element
                var dropdownRect = listItem.getBoundingClientRect();
                var dropdownWidth = target.getBoundingClientRect().width;

                // Calculate the right edge position
                var rightEdge = dropdownRect.right + dropdownWidth;
                var leftEdge = dropdownRect.left - dropdownWidth;

                if (html.getAttribute("dir") == "rtl") {
                    // Check if moving out to the right
                    if (e.classList.contains("child1")) {
                        if (dropdownRect.left < 0) {
                            elemList.clearNavDropdown = true;
                        }
                    }
                    if (leftEdge < 0) {
                        elemList.added.push(
                            target.previousElementSibling.getAttribute("data-id")
                        );
                    } else {
                        if (
                            listItem.closest("ul").classList.contains("force-left") &&
                            rightEdge < window.innerWidth
                        ) {
                            elemList.added.push(
                                target.previousElementSibling.getAttribute("data-id")
                            );
                        } else {
                            // Reset classes and position if not moving out
                            elemList.removed.push(
                                target.previousElementSibling.getAttribute("data-id")
                            );
                        }
                    }
                } else {
                    // Check if moving out to the right
                    if (e.classList.contains("child1")) {
                        if (dropdownRect.right > window.innerWidth) {
                            elemList.clearNavDropdown = true;
                        }
                    }
                    if (rightEdge > window.innerWidth) {
                        elemList.added.push(
                            target.previousElementSibling.getAttribute("data-id")
                        );
                    } else {
                        if (
                            listItem.closest("ul").classList.contains("force-left") &&
                            leftEdge > 0
                        ) {
                            elemList.added.push(
                                target.previousElementSibling.getAttribute("data-id")
                            );
                        }
                        // Check if moving out to the left
                        else if (leftEdge < 0) {
                            elemList.removed.push(
                                target.previousElementSibling.getAttribute("data-id")
                            );
                        } else {
                            elemList.removed.push(
                                target.previousElementSibling.getAttribute("data-id")
                            );
                        }
                    }
                }
            });
            let leftForceItem = document.querySelector(
                ".slide-menu.active.force-left"
            );
            if (leftForceItem) {
                if (document.querySelector("html").getAttribute("dir") != "rtl") {
                    let check = leftForceItem.getBoundingClientRect().right;
                    if (check < innerWidth) {
                        elemList.removed.push(
                            leftForceItem.previousElementSibling.getAttribute("data-id")
                        );
                    } else if (leftForceItem.getBoundingClientRect().left < 0) {
                        if (
                            document.documentElement.getAttribute("data-nav-style") ==
                            "menu-hover" ||
                            document.documentElement.getAttribute("data-nav-style") ==
                            "icon-hover" ||
                            window.innerWidth > 992
                        ) {
                            elemList.removed.push(
                                leftForceItem.previousElementSibling.getAttribute("data-id")
                            );
                        }
                    }
                } else {
                    let check =
                        leftForceItem.getBoundingClientRect().left -
                        leftForceItem.parentElement.closest(".slide-menu")?.clientWidth -
                        leftForceItem.getBoundingClientRect().width;
                    if (check > 0) {
                        if (
                            document.documentElement.getAttribute("data-nav-style") ==
                            "menu-hover" ||
                            document.documentElement.getAttribute("data-nav-style") ==
                            "icon-hover" ||
                            window.innerWidth > 992
                        ) {
                            elemList.removed.push(
                                leftForceItem.previousElementSibling.getAttribute("data-id")
                            );
                        }
                    }
                }
            }

            let elements = document.querySelectorAll(".main-menu .has-sub ul");
            elements.forEach((e) => {
                if (isElementVisible(e)) {
                    let ele = e.getBoundingClientRect();
                    if (document.documentElement.getAttribute("dir") == "rtl") {
                        if (ele.left < 0) {
                            if (e.classList.contains("child1")) {
                                elemList.removed.push(
                                    e.previousElementSibling.getAttribute("data-id")
                                );
                            } else {
                                elemList.added.push(
                                    e.previousElementSibling.getAttribute("data-id")
                                );
                            }
                        }
                    } else {
                        if (ele.right > innerWidth) {
                            if (e.classList.contains("child1")) {
                                elemList.removed.push(
                                    e.previousElementSibling.getAttribute("data-id")
                                );
                            } else {
                                elemList.added.push(
                                    e.previousElementSibling.getAttribute("data-id")
                                );
                            }
                        }
                    }
                }
            });
        }

        elemList.added = [...new Set(elemList.added)];
        elemList.removed = [...new Set(elemList.removed)];
        return elemList;
    },

    /**
     * **updateScrollVisibility**  
     * Escuta o evento de scroll e envia a posição do scroll para o Blazor via método assíncrono.
     * @param {Object} dotnetHelper - Referência ao objeto DotNet do Blazor para invocar métodos assíncronos.
     */
    updateScrollVisibility: function (dotnetHelper) {
        window.onscroll = function () {
            var scrollHeight = window.scrollY;
            dotnetHelper.invokeMethodAsync('UpdateScrollVisibility', scrollHeight);
        }
    },

    /**
     * **scrollToTop**  
     * Rola a página suavemente até o topo.
     */
    scrollToTop: function () {
        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    },

    /**
     * **registerScrollListener**  
     * Adiciona um listener ao evento de scroll e invoca o método Blazor assíncrono `SetStickyClass` com a posição do scroll.
     * @param {Object} dotnetHelper - Referência ao objeto DotNet do Blazor.
     */
    registerScrollListener: function (dotnetHelper) {
        window.addEventListener('scroll', function () {
            var scrollY = window.scrollY || window.pageYOffset;
            dotnetHelper.invokeMethodAsync("SetStickyClass", scrollY);
        });

        // Trigger initial check
        var scrollY = window.scrollY || window.pageYOffset;
        dotnetHelper.invokeMethodAsync("SetStickyClass", scrollY);
    },

    /**
     * **registerEscPressListener**  
     * Adiciona um listener ao evento de tecla para detectar o pressionamento da tecla ESC.  
     * Invoca o método Blazor `ClosePresentationMode` quando a tecla ESC é pressionada.
     * @param {Object} dotnetHelper - Referência ao objeto DotNet do Blazor.
     */
    registerEscPressListener: function (dotnetHelper) {
        this.escPressListener = function (e) {
            if (e.key === "Escape") {
                dotnetHelper.invokeMethodAsync("ClosePresentationMode");
            }
        };

        document.addEventListener('keydown', this.escPressListener);
    },

    /**
     * **removeEscPressListener**  
     * Remove o listener do evento de tecla ESC que foi previamente adicionado.
     */
    removeEscPressListener: function () {
        if (this.escPressListener) {
            document.removeEventListener('keydown', this.escPressListener);
            this.escPressListener = null; // Limpa a referência
        }
    },

    /**
     * **initializeTooltips**  
     * Inicializa os tooltips (dicas de ferramentas) usando Bootstrap.
     */
    initializeTooltips: function () {
        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        const tooltipList = [...tooltipTriggerList].map((tooltipTriggerEl) => new bootstrap.Tooltip(tooltipTriggerEl));
    },

    // remover tooltip, recebe id do elemento que contém o tooltip
    removeTooltips: function () {
        const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            const tooltipInstance = bootstrap.Tooltip.getInstance(tooltipTriggerEl);
            if (tooltipInstance) {
                tooltipInstance.hide();    // Esconde o tooltip
                tooltipInstance.dispose(); // Descarte a instância
                console.log("Tooltip escondido e descartado para:", tooltipTriggerEl);
            }
            // Remove 'aria-describedby' para desconectar o tooltip do elemento
            tooltipTriggerEl.removeAttribute('aria-describedby');
        });

        // Remover elementos .tooltip restantes do DOM
        const tooltips = document.querySelectorAll('.tooltip');
        tooltips.forEach(function (tooltipEl) {
            tooltipEl.remove();
            console.log("Elemento .tooltip removido:", tooltipEl);
        });
    },


    /**
     * **initializePopover**  
     * Inicializa os popovers usando Bootstrap.
     */
    initializePopover: function () {
        const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]');
        const popoverList = [...popoverTriggerList].map((popoverTriggerEl) => new bootstrap.Popover(popoverTriggerEl));
    },

    /**
     * **initCardRemove**  
     * Adiciona um evento para remover um cartão ao clicar em um botão de remoção associado.
     */
    initCardRemove: function () {
        let DIV_CARD = ".card";
        let cardRemoveBtn = document.querySelectorAll('[data-bs-toggle="card-remove"]');
        cardRemoveBtn.forEach((ele) => {
            ele.addEventListener("click", function (e) {
                e.preventDefault();
                let $this = this;
                let card = $this.closest(DIV_CARD);
                card.remove();
                return false;
            });
        });
    },

    /**
     * **initCardFullscreen**  
     * Adiciona um evento para alternar o modo de tela cheia em um cartão ao clicar em um botão de alternância associado.
     */
    initCardFullscreen: function () {
        let DIV_CARD = ".card";
        let cardFullscreenBtn = document.querySelectorAll('[data-bs-toggle="card-fullscreen"]');
        cardFullscreenBtn.forEach((ele) => {
            ele.addEventListener("click", function (e) {
                let $this = this;
                let card = $this.closest(DIV_CARD);
                card.classList.toggle("card-fullscreen");
                card.classList.remove("card-collapsed");
                e.preventDefault();
                return false;
            });
        });
    },

    /**
     * isThisElementVisible
     * @param {HTMLElement} element - O elemento HTML que será verificado.
     * @returns {boolean} - Retorna `true` se o elemento estiver visível (`display` diferente de `"none"`), caso contrário, retorna `false`.
     * Verifica se um elemento está visível no DOM, com base no valor calculado de sua propriedade `display`.
     **/
    isThisElementVisible(elementRef) {
        const selector = document.querySelector(elementRef);
        const computedStyle = window.getComputedStyle(selector);
        return computedStyle.display != "none";
    },
};

/**
 * **isElementVisible**  
 * @param {HTMLElement} element - O elemento HTML que será verificado.
 * @returns {boolean} - Retorna `true` se o elemento estiver visível (`display` diferente de `"none"`), caso contrário, retorna `false`.
 * Verifica se um elemento está visível no DOM, com base no valor calculado de sua propriedade `display`.
 **/
function isElementVisible(element) {
    const computedStyle = window.getComputedStyle(element);
    return computedStyle.display != "none";
} 