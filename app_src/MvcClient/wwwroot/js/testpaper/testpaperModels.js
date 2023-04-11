class Column {

    constructor(text, parentId, nodeId, nodes = []) {
        this.text = text;
        this.parentId = parentId;
        this.id = nodeId;
        this.nodes = nodes;
        //this.api_children = AppHost + '/api/columns/children/';
    }
}


class DataTree {
    constructor(container) {

        if (!$.fn['treeview']) {
            throw Error('plugin of treeview not found')
        }
        this.container = container
        this._treeeData = []
    }

    get treeData() {
        return this._treeeData;
    }
    set treeData(json) {
        this._treeeData = json;
    }
    api_cb(_node, result) {
        if (result && result.length > 0) {
            for (let index = 0; index < result.length; index++) {
                let item = result[index]
                $(this.container).treeview('addNode', [
                    _node.nodeId,
                    {
                        node: item
                    }
                ])
            }
        }
    }
    req_data(node) {
        if (node.nodes.length == 0) {
            let req = new HttpAction();
            req.request_data(req.tstppr_urls.创建栏目.apiChildren + node.id, null, r =>
                this.api_cb(node, r)
            )
        }
    }
    /**
     * 
     * @param {container id} eleId 
     */

    init() {

        let sf = this;
        $(this.container).treeview({
            data: sf._treeeData,
            state: {
                expanded: false
            },
            Extendible: true,
            onNodeSelected: function (event, data) {
                $(sf.container)
                    .parents('div')
                    .find('input.pid')
                    .val(data.text)
                $(sf.container)
                    .parents('div')
                    .find(':hidden[name=pid]')
                    .val(data.id)
                $('#collapseOne').collapse('toggle')
                return false
            },
            onNodeExpanded(event, data) {
                sf.req_data(data)
            }
        })

    }
}