CKEDITOR.editorConfig = function (config) {
    // Cấu hình cơ bản
    config.height = 300; // Chiều cao của CKEditor
    config.enterMode = CKEDITOR.ENTER_BR; // Nhấn Enter sẽ thêm thẻ <br>
    config.shiftEnterMode = CKEDITOR.ENTER_P; // Nhấn Shift+Enter sẽ thêm thẻ <p>

    // Tắt kiểm tra phiên bản
    config.versionCheck = false;

    // Tắt các thông báo
    config.removePlugins = 'notification,notificationaggregator'; // Tắt tất cả thông báo liên quan đến CKEditor

    // Tùy chỉnh toolbar hoặc các cấu hình khác nếu cần
    config.toolbarGroups = [
        { name: 'clipboard', groups: ['clipboard', 'undo'] },
        { name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
        { name: 'links' },
        { name: 'insert' },
        { name: 'forms' },
        '/',
        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
        { name: 'styles' },
        { name: 'colors' },
        { name: 'tools' },
        { name: 'about' }
    ];
};
