(function () {
    var trigger = document.getElementById("quiz-create-trigger");
    var quizCreateForm = document.getElementById('quiz-create-form');

    eventUtils.setEvent(trigger, 'click', function () {
        if (!this.className) {
            quizCreateForm.className = 'active-form';
            this.className = 'activated';
            this.innerHTML = 'Exit';
        } else {
            quizCreateForm.className = '';
            this.className = '';
            this.innerHTML = 'Edit Quiz';
        }
    });

    eventUtils.setEvent(quizCreateForm, 'submit', function (evt) {
        if (!javascriptValidator.validateContainerElement(this, 2)) {
            eventUtils.preventDefault(evt);
            return;
        }

       repository.quizzes.update(htmlUtils.extractFormData(this)).then(
            function (s) {
                htmlUtils.overlayMessage({ toEl: quizCreateForm, autoHide: true, update: true, level: 10 });
            }, function (e) {
                htmlUtils.overlayMessage({ toEl:quizCreateForm, isError: true, message : e, update : true, level : 10 });
            });

        var loader = document.createElement('div');
        loader.className = 'loader';

        htmlUtils.showInOverlayBox(loader, 10, this.parentElement);
        eventUtils.preventDefault(evt);
    });
}())