/**
 * generate doms * 
 */
class DomGen {

    /**      
     * @param {Array } array_inputs 
     * @param { string } outer_shell 
     */
    constructor(array_inputs, outer_shell = 'div') {
        this.array_inputs = array_inputs;
        this.outer_shell = outer_shell;
    }

    /**
     * append dom by juqery
     * @param {outer ele dom id/class} outerEle 
     * @param {element needed} ele 
     * @param {*} jsonAttrs 
     * @param {*} txt 
     */
    static appendDom = (outerEle, ele, jsonAttrs, txt) => {
        let e = DomGen.createDom(ele, jsonAttrs, txt);
        $(outerEle).append(e);
        return outerEle;
    }

    static createDom(ele, jsonAttrs, txt) {  //tips:If just want html string, use 'dom.outerHTML'.
        let e = document.createElement(ele);
        if (jsonAttrs) {
            for (var k in jsonAttrs) {
                e.setAttribute(k, jsonAttrs[k]);
            }
        }

        if (txt) {
           
            if (ele == 'input') {
                e.value = txt;
            } else {
                e.innerText = txt;
            }
        }
        return e;
    }

    genButton(txt, btnCls) {
        let btn = DomGen.createDom('button', { 'class': btnCls || 'btn btn-primary btn-xs' }, txt || '提交');
        return btn;
    }

    genFormInput(cls, value) {
        return DomGen.createDom('input', { 'class': cls, 'value': value });
    }


    /**
     * 
     * @param {String} id: dom id 
     * @param {String} cls: dom class
     * @param { Array } array_titles :values of the inputs in order
     * @param {URL} submitUrl : post url
     * @param {Boolean} needBtn 
     */
    genForm(id, cls, array_titles, submitUrl = '/', needBtn = false) {
        let
            outer = DomGen.createDom(this.outer_shell, { 'id': id, 'class': cls || id }),
            innerContainer = '',
            titleContainer = '';

        outer.setAttribute('action', submitUrl);
        outer.setAttribute('method', "post");
        for (let i = 0; i < this.array_inputs.length; i++) {
            let temp_input = this.array_inputs[i];
            innerContainer = DomGen.createDom('div', { 'class': 'input-group' });
            titleContainer = DomGen.createDom('span', { 'class': 'input-group-addon' }, array_titles[i]);
            innerContainer.append(titleContainer);
            let form_control = DomGen.createDom('input',
                {
                    'type': temp_input['type'],
                    'name': temp_input['name'] || '',
                    'value': temp_input['value'] || '',
                    'class': temp_input['class'] || '',
                    'onchange': temp_input['onchange'] || '',
                    'required': 'required'
                }),

                is_id = String(temp_input['class']).toLowerCase().split(' ').findIndex(function (x) {
                    return x === 'id';
                }),
                is_maker = String(temp_input['class']).toLowerCase().split(' ').findIndex(function (x) {
                    return x === 'maker';
                }),

                is_mobile = String(temp_input['class']).toLowerCase().split(' ').findIndex(function (x) {
                    return x === 'mobile';
                });

            if (is_id !== -1 || is_mobile !== -1 || is_maker !== -1) {
                form_control.disabled = true;
            }


            innerContainer.append(form_control);
            outer.append(innerContainer);
        }
        if (needBtn) { outer.append(this.genButton()); }
        return outer;
    }

    /**
     * create table row with right field value
     * @param {*} row_data 
     * @param { table header column names} table_header
     * @param {*} btns 
     * @param {*} hasChkbox 
     */
    static create_tr_by_name(r_data, table_header, btns, hasChkbox) {

        let _self = this,
            tr = DomGen.createDom('tr', { 'data-id': r_data["UserID"] || r_data['Id'] });
        if (hasChkbox) {
            tr.append(DomGen.create_chk_tdf());
        }

        let _header = '', r_indx;
        for (let i = 0; i < table_header.length; i++) {
            _header = table_header[i];
            if (!_header) {
                continue;
            }

            r_indx = Object.keys(r_data).findIndex(x => {
                return x.toLowerCase() === _header.toLowerCase();
            })

            if (r_indx != -1) {
                if (_header.toLowerCase() == 'gender') {
                    r_data[_header] = r_data[_header] ? '男' : '女';
                }

                if (_header.toLowerCase() == 'updatetime') {
                    r_data[_header] = new Date(r_data[_header]).toLocaleDateString();
                }

                tr.append(DomGen.createDom('td', { 'name': _header }, r_data[_header] || '**'));

            } else {
                continue;
            }

        }
        if (btns.length > 0) {
            tr.append(_self.create_tr_btns(btns))
        }
        return tr;
    }

    static create_chk_tdf = function () {
        let
            ckarr = { 'type': 'checkbox' },
            ck = DomGen.createDom('input', ckarr),
            td = DomGen.createDom('td');
        td.append(ck);
        return td;
    }

    static create_tr_btns = function (btn_array) {
        let
            td = DomGen.createDom('td');
        for (let x = 0; x < btn_array.length; x++) {
            let
                btn = btn_array[x],
                a_bj = DomGen.createDom('a', { 'class': btn.class, 'onclick': btn.event, 'href': "javascript:;" }, btn.text);
            td.append(a_bj);
        }
        return td;
    }

    //create table row by key value pair and row buttons
    genTable(json, theads_array, btn_array, genck) {
        let _self = this,
            r = document.createElement('div');

        $.each(json, function (k, v) {
            if (v) {
                r.append(_self.create_tr_by_name(v, theads_array, btn_array || [], genck));
            }
        })

        return r.innerHTML;
    }

}

